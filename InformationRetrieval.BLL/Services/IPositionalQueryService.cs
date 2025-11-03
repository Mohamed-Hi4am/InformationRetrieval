using InformationRetrieval.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval.BLL.Services
{
    public interface IPositionalQueryService
    {
        List<string> ExecutePhraseQuery(string phrase, PositionalIndex indexData, List<string> documentNames, ITextProcessorService tokenizer);
    }
}
