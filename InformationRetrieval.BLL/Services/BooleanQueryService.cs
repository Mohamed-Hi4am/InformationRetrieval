using InformationRetrieval.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval.BLL.Services
{
    public class BooleanQueryService : IBooleanQueryService
    {
        public List<string> ExecuteQuery(QueryModel query, InvertedIndex indexData, List<string> documentNames)
        {
            // Get the list of document IDs for the first term.
            indexData.Index.TryGetValue(query.Term1?.ToLower().Trim() ?? "", out var postings1);
            postings1 ??= new List<int>();

            List<int> resultDocIds;

            switch (query.Operator.ToUpper())
            {
                case "AND":
                    indexData.Index.TryGetValue(query.Term2?.ToLower().Trim() ?? "", out var postings2);
                    postings2 ??= new List<int>();
                    resultDocIds = postings1.Intersect(postings2).ToList();
                    break;
                case "OR":
                    indexData.Index.TryGetValue(query.Term2?.ToLower().Trim() ?? "", out var postings2_or);
                    postings2_or ??= new List<int>();
                    resultDocIds = postings1.Union(postings2_or).ToList();
                    break;
                case "NOT":
                    var allDocIds = Enumerable.Range(0, documentNames.Count).ToList();
                    resultDocIds = allDocIds.Except(postings1).ToList();
                    break;
                default:
                    resultDocIds = postings1; // Default to a single term search.
                    break;
            }

            // Convert the resulting document IDs back to document names.
            return resultDocIds.OrderBy(id => id).Select(id => documentNames[id]).ToList();
        }
    }
}
