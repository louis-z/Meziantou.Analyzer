using System;
using System.Text;
using System.Threading.Tasks;
using Meziantou.Analyzer.Rules;
using TestHelper;
using Xunit;

namespace Meziantou.Analyzer.Test.Rules
{
    public sealed class UseProperSpellingTests
    {
        private readonly string[] _knownMisspelledWordVariations = new[]
        {
            "adress",
            "adresses",
            "adressing",
            "catched",
            "checkouted",
            "checkouting",
            "developper",
            "developpers",
            "developping",
            "setuped",
            "setuping",
            "setupped",
            "setupping",
            "succes",
            "succesful",
            "succesfull",
            "sucess",
            "sucessful",
            "sucessfull",
        };

        private readonly Random _random = new Random();

        private static ProjectBuilder CreateProjectBuilder()
        {
            return new ProjectBuilder()
                .WithAnalyzer<UseProperSpellingAnalyzer>();
        }

        [Fact]
        public async Task Test_CommentsContainKnownMisspellings_DiagnosticIsReportedForEachOne()
        {
            var sourceCode = @"
class TestClass
{
    void Test()
    {
        // Most [|developpers|] would not have written the following. Is this the work of a JUNIOR [|DEVELOPPER|]?
        if (true)
            return;
        /*
        We should [|adress|] the problem of [|checkouted|] files and [|setuping|]
(or is it spelled [|setupping|]?) them. If not [|sucessfull|], throw an exception
    that will be [|catched|] somewhere else.
        */
    }
}";
            await CreateProjectBuilder()
                  .WithSourceCode(sourceCode)
                  .ValidateAsync();
        }

        [Fact]
        public async Task Test_ValidateAllKnownMisspellingVariations_DiagnosticIsReportedForEachOne()
        {
            var sb = new StringBuilder(@"
class TestClass
{
    void Test()
    {
");
            foreach (var word in _knownMisspelledWordVariations)
            {
                var array = word.ToCharArray();
                for (var i = 0; i < array.Length; i++)
                {
                    array[i] = ToRandomCase(array[i]);
                }
                sb.Append("        // [|").Append(array).AppendLine("|] ");
            }

            sb.Append(@"        }
}");
            var sourceCode = sb.ToString();

            await CreateProjectBuilder()
                  .WithSourceCode(sourceCode)
                  .ValidateAsync();
        }

        private char ToRandomCase(char c)
        {
            return (_random.Next() % 2 == 0) ?
                char.ToUpper(c) :
                char.ToLower(c);
        }
    }
}
