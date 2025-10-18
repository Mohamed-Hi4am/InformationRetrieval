using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval.BLL.Models
{
    public class InvertedIndex
    {
        // Use the concrete type you want
        public SortedDictionary<string, List<int>> Index { get; set; }
    }
}
