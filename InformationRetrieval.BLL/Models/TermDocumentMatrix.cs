using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval.BLL.Models
{
    public class TermDocumentMatrix
    {
        public List<string> Terms { get; set; } // The sorted vocabulary
        public List<string> DocumentNames { get; set; }
        public List<List<int>> Incidence { get; set; } // The 0s and 1s (The 2D array: matrix)
    }
}
