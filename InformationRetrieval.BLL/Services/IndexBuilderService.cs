using InformationRetrieval.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval.BLL.Services
{
    public class IndexBuilderService : IIndexBuilderService
    {
        private readonly ITextProcessorService _textProcessor;

        public IndexBuilderService(ITextProcessorService textProcessor)
        {
            _textProcessor = textProcessor;
        }

        public ProcessingResult Build(Dictionary<string, string> documents)
        {
            var documentNames = documents.Keys.ToList();
            var allTokensByDoc = new List<List<string>>();
            var vocabulary = new HashSet<string>();

            // 1. Tokenize all documents and build the vocabulary
            foreach (var docContent in documents.Values)
            {
                var tokens = _textProcessor.Tokenize(docContent);
                allTokensByDoc.Add(tokens);
                foreach (var token in tokens)
                {
                    vocabulary.Add(token);
                }
            }

            var sortedVocabulary = vocabulary.OrderBy(term => term).ToList();
            var termLookup = sortedVocabulary.Select((term, index) => new { term, index })
                                             .ToDictionary(t => t.term, t => t.index);

            // 2. Build the Term-Document Incidence Matrix
            var incidenceMatrix = new List<List<int>>();
            foreach (var term in sortedVocabulary)
            {
                var row = new List<int>();
                foreach (var docTokens in allTokensByDoc)
                {
                    row.Add(docTokens.Contains(term) ? 1 : 0);
                }
                incidenceMatrix.Add(row);
            }

            // 3. Build the Inverted Index
            var invertedIndexDict = new Dictionary<string, List<int>>();
            for (int docIndex = 0; docIndex < allTokensByDoc.Count; docIndex++)
            {
                foreach (var token in allTokensByDoc[docIndex].Distinct())
                {
                    if (!invertedIndexDict.ContainsKey(token))
                    {
                        invertedIndexDict[token] = new List<int>();
                    }
                    invertedIndexDict[token].Add(docIndex);
                }
            }

            // 4. Assemble the final result object
            var result = new ProcessingResult
            {
                Matrix = new TermDocumentMatrix
                {
                    Terms = sortedVocabulary,
                    DocumentNames = documentNames,
                    Incidence = incidenceMatrix
                },
                Index = new InvertedIndex
                {
                    // The dictionary should also be sorted by term for consistency
                    Index = new SortedDictionary<string, List<int>>(invertedIndexDict)
                }
            };

            return result;
        }
    }
}
