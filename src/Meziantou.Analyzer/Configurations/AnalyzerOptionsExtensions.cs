using System;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Meziantou.Analyzer.Configurations
{
    public static class AnalyzerOptionsExtensions
    {
        public static bool GetConfigurationValue(this AnalyzerOptions options, SyntaxTree tree, string key, bool defaultValue)
        {
            var analyzerConfigOptions = options.AnalyzerConfigOptionsProvider.GetOptions(tree);
            if (analyzerConfigOptions.TryGetValue(key, out var value))
            {
                return ChangeType(value, defaultValue);
            }

            return defaultValue;
        }

        public static bool? GetConfigurationValue(this AnalyzerOptions options, SyntaxTree tree, string key, bool? defaultValue)
        {
            var analyzerConfigOptions = options.AnalyzerConfigOptionsProvider.GetOptions(tree);
            if (analyzerConfigOptions.TryGetValue(key, out var value))
            {
                return ChangeType(value, defaultValue);
            }

            return defaultValue;
        }

        public static int GetConfigurationValue(this AnalyzerOptions options, SyntaxTree tree, string key, int defaultValue)
        {
            var analyzerConfigOptions = options.AnalyzerConfigOptionsProvider.GetOptions(tree);
            if (analyzerConfigOptions.TryGetValue(key, out var value))
            {
                return ChangeType(value, defaultValue);
            }

            return defaultValue;
        }

        public static ReportDiagnostic? GetConfigurationValue(this AnalyzerOptions options, SyntaxTree tree, string key, ReportDiagnostic? defaultValue)
        {
            var analyzerConfigOptions = options.AnalyzerConfigOptionsProvider.GetOptions(tree);
            if (analyzerConfigOptions.TryGetValue(key, out var value))
            {
                if (value != null && Enum.TryParse<ReportDiagnostic>(value, ignoreCase: true, out var result))
                    return result;
            }

            return defaultValue;
        }

        public static bool TryGetConfigurationValue(this AnalyzerOptions options, SyntaxTree tree, string key, out string value)
        {
            var analyzerConfigOptions = options.AnalyzerConfigOptionsProvider.GetOptions(tree);
            return analyzerConfigOptions.TryGetValue(key, out value);
        }

        public static bool TryGetConfigurationValue(this AnalyzerOptions options, IOperation operation, string key, out string value)
        {
            return TryGetConfigurationValue(options, operation.Syntax, key, out value);
        }

        public static bool TryGetConfigurationValue(this AnalyzerOptions options, SyntaxNode syntaxNode, string key, out string value)
        {
            return TryGetConfigurationValue(options, syntaxNode.SyntaxTree, key, out value);
        }

        private static bool ChangeType(string value, bool defaultValue)
        {
            if (value != null && bool.TryParse(value, out var result))
                return result;

            return defaultValue;
        }

        private static bool? ChangeType(string value, bool? defaultValue)
        {
            if (value != null && bool.TryParse(value, out var result))
                return result;

            return defaultValue;
        }

        private static int ChangeType(string value, int defaultValue)
        {
            if (value != null && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                return result;

            return defaultValue;
        }
    }
}
