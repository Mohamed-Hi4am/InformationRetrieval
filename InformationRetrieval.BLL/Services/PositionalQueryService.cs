using InformationRetrieval.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval.BLL.Services
{
    public class PositionalQueryService : IPositionalQueryService
    {
        public List<string> ExecutePhraseQuery(string phrase, PositionalIndex indexData, List<string> documentNames, ITextProcessorService tokenizer)
        {
            // Handle empty or null phrase
            if (string.IsNullOrWhiteSpace(phrase))
            {
                return new List<string>();
            }

            // Tokenize the phrase using the same rules as document tokenization
            var terms = tokenizer.Tokenize(phrase);

            // Handle empty tokenization result
            if (terms == null || terms.Count == 0)
            {
                return new List<string>();
            }

            // Single term phrase - return documents containing the term
            if (terms.Count == 1)
            {
                var term = terms[0];
                if (indexData.Index.TryGetValue(term, out var postings))
                {
                    return postings.Keys.OrderBy(docId => docId).Select(docId => documentNames[docId]).ToList();
                }
                return new List<string>();
            }

            // Multi-term phrase - use positional intersection
            var firstTerm = terms[0];
            
            // Get postings for first term
            if (!indexData.Index.TryGetValue(firstTerm, out var currentPostings))
            {
                return new List<string>(); // First term not found
            }

            // Initialize with positions from first term
            var resultPostings = new Dictionary<int, List<int>>(currentPostings);

            // Process each subsequent term
            for (int termIndex = 1; termIndex < terms.Count; termIndex++)
            {
                var nextTerm = terms[termIndex];
                
                // Get postings for next term
                if (!indexData.Index.TryGetValue(nextTerm, out var nextPostings))
                {
                    return new List<string>(); // Term not found, no matches possible
                }

                // Perform positional intersection
                var mergedPostings = new Dictionary<int, List<int>>();

                // Check only documents that have both terms
                foreach (var docId in resultPostings.Keys.Intersect(nextPostings.Keys))
                {
                    var currentPositions = resultPostings[docId];
                    var nextPositions = nextPostings[docId];

                    // Find positions in next term that follow positions in current term
                    var validNextPositions = new List<int>();

                    foreach (var currentPos in currentPositions)
                    {
                        var expectedNextPos = currentPos + 1;
                        if (nextPositions.Contains(expectedNextPos))
                        {
                            validNextPositions.Add(expectedNextPos);
                        }
                    }

                    // If we found consecutive positions, add to merged results
                    if (validNextPositions.Count > 0)
                    {
                        mergedPostings[docId] = validNextPositions;
                    }
                }

                resultPostings = mergedPostings;

                // Early exit if no documents match so far
                if (resultPostings.Count == 0)
                {
                    return new List<string>();
                }
            }

            // Convert document IDs to document names
            return resultPostings.Keys.OrderBy(docId => docId).Select(docId => documentNames[docId]).ToList();
        }
    }
}
