using InformationRetrieval.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval.BLL.Services
{
    public interface IBooleanQueryService
    {
        List<string> ExecuteQuery(QueryModel query, InvertedIndex index, List<string> docNames);
    }
}
