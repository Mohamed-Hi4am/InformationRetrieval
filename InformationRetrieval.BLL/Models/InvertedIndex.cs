using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval.BLL.Models
{
    public class InvertedIndex
    {
        // Key: Term, Value: a list of the documents ID's it appeared in
        public SortedDictionary<string, List<int>> Index { get; set; }
    }
}
