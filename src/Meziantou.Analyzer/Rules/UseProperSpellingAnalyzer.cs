using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Meziantou.Analyzer.Rules
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseProperSpellingAnalyzer : DiagnosticAnalyzer
    {
        private const string B = @"\b"; // Word Boundary
        private static readonly WordFinder[] s_misspelledWordFinders = new[]
        {
            new WordFinder($@"{B}adress(?<suffix>(es|ing)?){B}"),
            new WordFinder($@"{B}catched{B}"),
            new WordFinder($@"{B}checkout(?<suffix>(ed|ing)){B}"),
            new WordFinder($@"{B}developp(?<suffix>(er|ers|ing)){B}"),
            new WordFinder($@"{B}setup(?<suffix>(ed|ing|ped|ping)){B}"),
            new WordFinder($@"{B}suc(ess|ces)(ful|full)?{B}"),
        };

        private static readonly DiagnosticDescriptor s_rule = new DiagnosticDescriptor(
            RuleIdentifiers.UseProperSpelling,
            title: "Use proper spelling",
            messageFormat: "Known misspelling: '{0}'",
            RuleCategories.Naming,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: "",
            helpLinkUri: RuleIdentifiers.GetHelpUri(RuleIdentifiers.UseProperSpelling));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(s_rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxTreeAction(AnalyzeCommentSpelling);
        }

        private static void AnalyzeCommentSpelling(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            var allCommentTrivia = root.DescendantTrivia().Where(trivia =>
                trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) || trivia.IsKind(SyntaxKind.SingleLineCommentTrivia));

            foreach (var commentTrivia in allCommentTrivia)
            {
                var comment = commentTrivia.ToString();
                foreach (var wordFinder in s_misspelledWordFinders)
                {
                    var matches = wordFinder.Matches(comment);
                    foreach (Match match in matches)
                    {
                        var location = commentTrivia.SyntaxTree.GetLocation(new TextSpan(commentTrivia.SpanStart + match.Index, match.Length));
                        var diagnostic = Diagnostic.Create(s_rule, location, match.ToString());
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}
