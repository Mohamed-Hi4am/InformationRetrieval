using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval.BLL.Services
{
    public interface ITextProcessorService
    {
        List<string> Tokenize(string text);
    }
}
