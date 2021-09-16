using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using Gint.Markup;

namespace Gint.Tests
{
    public class MarkupTests
    {
        [Fact]
        //TODO: add individual tests
        public void Markup_Lexer()
        {
            string Print(IEnumerable<MarkupSyntaxToken> tokens)
            {
                return tokens.Select(c => c.ToString()).Aggregate((fst, snd) => $"{fst}{Environment.NewLine}{snd}");
            }

            var tokens1 = MarkupLexer.Tokenize("hello [bold]world", out var diagnostics1);
            var res1 =Print(tokens1);

            var tokens2 = MarkupLexer.Tokenize("hello [bold]world[-bold]", out var diagnostics2);
            var res2= Print(tokens2);

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

    }
}
