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

            // 3. Build the Inverted Index and Positional Index simultaneously
            var invertedIndexDict = new Dictionary<string, List<int>>();
            var positionalIndexDict = new Dictionary<string, Dictionary<int, List<int>>>();
            
            for (int docIndex = 0; docIndex < allTokensByDoc.Count; docIndex++)
            {
                var tokens = allTokensByDoc[docIndex];
                for (int position = 0; position < tokens.Count; position++)
                {
                    var token = tokens[position];
                    
                    // Build inverted index (unique document IDs per term)
                    if (!invertedIndexDict.ContainsKey(token))
                    {
                        invertedIndexDict[token] = new List<int>();
                    }
                    if (!invertedIndexDict[token].Contains(docIndex))
                    {
                        invertedIndexDict[token].Add(docIndex);
                    }
                    
                    // Build positional index (positions per document per term)
                    if (!positionalIndexDict.ContainsKey(token))
                    {
                        positionalIndexDict[token] = new Dictionary<int, List<int>>();
                    }
                    if (!positionalIndexDict[token].ContainsKey(docIndex))
                    {
                        positionalIndexDict[token][docIndex] = new List<int>();
                    }
                    positionalIndexDict[token][docIndex].Add(position);
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
                },
                PositionalIndex = new PositionalIndex
                {
                    Index = new SortedDictionary<string, Dictionary<int, List<int>>>(positionalIndexDict)
                }
            };

            return result;
        }
    }
}
