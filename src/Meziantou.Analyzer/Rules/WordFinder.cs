using System;
using System.Text.RegularExpressions;

namespace Meziantou.Analyzer.Rules
{
    internal sealed class WordFinder : Regex
    {
        public WordFinder(string pattern)
            : base(pattern,
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
                TimeSpan.FromSeconds(1))
        {
        }
    }
}
