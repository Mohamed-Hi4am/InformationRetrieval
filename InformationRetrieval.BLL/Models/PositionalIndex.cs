using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval.BLL.Models
{
    public class PositionalIndex
    {
        // Key: the Term
        // Value: Dictionary where:
        //   - Key: Document ID (index)
        //   - Value: List of positions where the term appears in that document
        public SortedDictionary<string, Dictionary<int, List<int>>> Index { get; set; }
    }
}
