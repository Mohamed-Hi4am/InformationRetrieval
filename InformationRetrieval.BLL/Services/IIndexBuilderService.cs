using InformationRetrieval.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval.BLL.Services
{
    public interface IIndexBuilderService
    {
        ProcessingResult Build(Dictionary<string, string> documents);
    }
}
