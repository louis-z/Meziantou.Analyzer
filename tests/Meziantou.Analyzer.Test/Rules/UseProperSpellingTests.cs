using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Meziantou.Analyzer.Rules;
using TestHelper;
using Xunit;

namespace Meziantou.Analyzer.Test.Rules;

public sealed class UseProperSpellingTests
{
    private static readonly string[] _knownMisspellings = new[]
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
        "occured",
        "occuring",
        "ocur",
        "ocured",
        "ocuring",
        "ocurred",
        "ocurring",
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

    public static IEnumerable<object[]> GetKnownMisspellings()
    {
        foreach (var word in _knownMisspellings)
            yield return new[] { word };
    }

    private static ProjectBuilder CreateProjectBuilder()
    {
        return new ProjectBuilder()
            .WithAnalyzer<UseProperSpellingAnalyzer>();
    }

    [Fact]
    public async Task Test_CommentsWithKnownMisspellings_DiagnosticReportedForEach()
    {
        var sourceCode = @"
class TestClass
{
    void Test()
    {
        // Most [|developpers|] would not have written the following. Is this the work of a JUNIOR DEVELOPPER?
        if (true)
            return;
        /*
        We should [|adress|] the checkout problem of [|checkouted|] files and [|setuping|]
(or is it spelled [|setupping|]?) them. If not [|sucessfull|], throw an exception
    that will be [|catched|] somewhere else.
        */
    }
}";
        await CreateProjectBuilder()
              .WithSourceCode(sourceCode)
              .ValidateAsync();
    }

    [Theory]
    [MemberData(nameof(GetKnownMisspellings))]
    public async Task Test_KnownMisspellings_DiagnosticReportedForEach(string misspelledWord)
    {
        var sb = new StringBuilder(@"
class TestClass
{
    void Test()
    {
");
        var capitalized = char.ToUpperInvariant(misspelledWord[0]) + misspelledWord.Substring(1);

        sb.Append("        // [|").Append(misspelledWord).AppendLine("|]");
        sb.Append("        // [|").Append(misspelledWord).AppendLine("|]Suffix");
        sb.Append("        // ").Append("prefix[|").Append(capitalized).AppendLine("|]");
        sb.Append("        // ").Append("prefix[|").Append(capitalized).AppendLine("|]Suffix");
        sb.Append(@"    }
}");
        var sourceCode = sb.ToString();

        await CreateProjectBuilder()
              .WithSourceCode(sourceCode)
              .ValidateAsync();
    }

    [Theory]
    [MemberData(nameof(GetKnownMisspellings))]
    public async Task Test_KnownMisspellingOutOfContext_NoDiagnosticReported(string misspelledWord)
    {
        var sb = new StringBuilder(@"
class TestClass
{
    void Test()
    {
");
        var capitalized = char.ToUpperInvariant(misspelledWord[0]) + misspelledWord.Substring(1);
        var maybeAcronym = misspelledWord.ToUpperInvariant();

        sb.Append("        // ").Append(maybeAcronym).AppendLine("");
        sb.Append("        // ").Append(misspelledWord).AppendLine("suffix");
        sb.Append("        // ").Append("prefix").AppendLine(misspelledWord);
        sb.Append("        // ").Append("prefix").Append(capitalized).AppendLine("suffix");
        sb.Append(@"    }
}");
        var sourceCode = sb.ToString();

        await CreateProjectBuilder()
              .WithSourceCode(sourceCode)
              .ValidateAsync();
    }
}
