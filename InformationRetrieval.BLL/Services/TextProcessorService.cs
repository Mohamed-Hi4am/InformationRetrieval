using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InformationRetrieval.BLL.Services
{
    public class TextProcessorService : ITextProcessorService
    {
        public List<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<string>();
            }

            // 1. Normalize to lowercase as per the requirements.
            var lowerCaseText = text.ToLower();

            // 2. Handle hyphenated words by replacing hyphens with spaces.
            // This ensures "state-of-the-art" becomes "state of the art" before splitting.
            var processedText = lowerCaseText.Replace('-', ' ');

            // 3. Define the tokenization regex.
            // This pattern finds sequences of letters and numbers,
            // including those with periods in between (like "u.s.a.").
            var regex = new Regex(@"[a-z0-9]+(?:\.[a-z0-9]+)*");

            // 4. Find all matches and convert them to a list of strings.
            var tokens = regex.Matches(processedText)
                              .Cast<Match>()
                              .Select(m => m.Value)
                              .ToList();

            return tokens;
        }
    }
}
