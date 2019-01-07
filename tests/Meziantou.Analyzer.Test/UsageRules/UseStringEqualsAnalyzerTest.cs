﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Meziantou.Analyzer.Test
{
    [TestClass]
    public class UseStringEqualsAnalyzerTest : CodeFixVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new UseStringEqualsAnalyzer();

        [TestMethod]
        public void Equals_ShouldNotReportDiagnosticForEmptyString()
        {
            var test = "";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void Equals_StringLiteral_stringLiteral_ShouldReportDiagnostic()
        {
            var test = @"
class TypeName
{
    public void Test()
    {
        var a = ""a"" == ""v"";
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "MA0006",
                Message = "Use string.Equals instead of Equals operator",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", line: 6, column: 17)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void Equals_StringVariable_stringLiteral_ShouldReportDiagnostic()
        {
            var test = @"
class TypeName
{
    public void Test()
    {
        string str = "";
        var a = str == ""v"";
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "MA0006",
                Message = "Use string.Equals instead of Equals operator",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", line: 7, column: 17)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void Equals_ObjectVariable_stringLiteral_ShouldReportDiagnostic()
        {
            var test = @"
class TypeName
{
    public void Test()
    {
        object str = "";
        var a = str == ""v"";
    }
}";

            VerifyCSharpDiagnostic(test);
        }
    }
}