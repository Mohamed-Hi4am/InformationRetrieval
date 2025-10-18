using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval.BLL.Models
{
    public class QueryModel
    {
        public string Term1 { get; set; }
        public string Term2 { get; set; } // Not used for NOT queries
        public string Operator { get; set; } // AND, OR, NOT
    }
}
