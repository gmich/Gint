using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using Gint.Markup;
using Xunit.Abstractions;

namespace Gint.Tests
{
    public class MarkupTests
    {
        private readonly ITestOutputHelper output;

        public MarkupTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        //TODO: add individual tests
        public void Markup_Lexer()
        {
            string Print(IEnumerable<MarkupSyntaxToken> tokens)
            {
                var res = tokens.Select(c => c.ToString()).Aggregate((fst, snd) => $"{fst}{Environment.NewLine}{snd}");
                output.WriteLine(res);
                return res;
            }

            var tokens1 = MarkupLexer.Tokenize("hello [bold]world", out var diagnostics1);
            var res1 = Print(tokens1);

            var tokens2 = MarkupLexer.Tokenize("hello [bold]world[-bold]", out var diagnostics2);
            var res2 = Print(tokens2);

            var tokens3 = MarkupLexer.Tokenize("hello [bold,fg:red] world [-bold,fg:red]", out var diagnostics3);
            var res3 = Print(tokens3);

            var tokens4 = MarkupLexer.Tokenize("hello [[bold] world", out var diagnostics4);
            var res4 = Print(tokens4);

            var tokens5 = MarkupLexer.Tokenize("hello [", out var diagnostics5);
            var res5 = Print(tokens5);

            var tokens6 = MarkupLexer.Tokenize("hello [bold", out var diagnostics6);
            var res6 = Print(tokens6);

            MarkupLinter.Lint("hello [bold,fg:red] world [-bold,fg:red]", out var lintdiagnostics1);
            MarkupLinter.Lint("hello [bold,fg:red] world [-bold,-fg:red]", out var lintdiagnostics2);
        }

        [Fact]
        //TODO: add individual tests
        public void Markup_Linter()
        {
            MarkupLinter.Lint("hello [bold,fg:red] world [-bold,fg:red]", out var lintdiagnostics1);
            MarkupLinter.Lint("hello [bold,fg:red] world [-bold,-fg:red]", out var lintdiagnostics2);
        }

        [Fact]
        //TODO: add individual tests
        public void Markup_Console_Writer()
        {
            new ConsoleMarkupWriter().Print("hello [bold,fg:red] world [-bold,fg:red,-bold]");
            new ConsoleMarkupWriter().Print("hello [bold,fg:red] world [-bold,-fg:red]");
        }

        private void PrintDiagnostics(Markup.DiagnosticCollection diagnostics, string text)
        {
            foreach (var diagnostic in diagnostics)
            {
                var error = text.Substring(diagnostic.Location.Start, diagnostic.Location.Length);
                var prefix = text.Substring(0, diagnostic.Location.Start);
                var suffix = text[diagnostic.Location.End..];
                output.WriteLine($"{prefix}-->{error}<--{suffix}");
                output.WriteLine(diagnostic.Message);
                output.WriteLine(string.Empty);
            }
        }

    }
}
