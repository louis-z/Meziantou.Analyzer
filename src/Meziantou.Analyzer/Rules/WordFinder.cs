using System;
using System.Text.RegularExpressions;

namespace Meziantou.Analyzer.Rules
{
    internal sealed class WordFinder : Regex
    {
        private const string WordBoundaryBehind = @"(?<=\b)";
        private const string OptionalSuffixAhead = @"(?=\b|[A-Z0-9_])";
        private const string OptionalPrefixBehind = @"(?<=\b|[a-z0-9_])";

        private static string BuildRegexPatternFor(string word)
        {
            var capitalized = char.ToUpperInvariant(word[0]) + word.Substring(1);
            return $@"{WordBoundaryBehind}{word}{OptionalSuffixAhead}|" +
                   $@"{OptionalPrefixBehind}{capitalized}{OptionalSuffixAhead}";
        }

        public WordFinder(string word)
            : base(BuildRegexPatternFor(word),
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
                TimeSpan.FromSeconds(1))
        {
        }
    }
}
