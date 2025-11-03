using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval.BLL.Models
{
    public class ProcessingResult
    {
        public TermDocumentMatrix Matrix { get; set; }
        public InvertedIndex Index { get; set; }
        public PositionalIndex PositionalIndex { get; set; }
    }
}
