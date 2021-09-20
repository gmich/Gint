using Gint.Markup;
using System.Linq;
using Xunit;
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

        private void PrintDiagnostics(DiagnosticCollection diagnostics, string text)
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

        private static void EqualsKindAndValue(MarkupSyntaxToken token, MarkupTokenKind kind, string value = null)
        {
            Assert.Equal(kind, token.Kind);
            Assert.Equal(value, token.Value);
        }

        [Fact]
        [Trait("Markup", "Lexer")]
        public void FormatStart_Text_FormatEnd()
        {
            var tokens = MarkupLinter.Lint("[bold]hello[-bold]", out var diagnostics);

            Assert.Empty(diagnostics);
            Assert.Equal(3, tokens.Length);

            EqualsKindAndValue(tokens[0], MarkupTokenKind.FormatStart, "bold");
            EqualsKindAndValue(tokens[1], MarkupTokenKind.Text, "hello");
            EqualsKindAndValue(tokens[2], MarkupTokenKind.FormatEnd, "bold");
        }

        [Fact]
        [Trait("Markup", "Lexer")]
        public void FormatStart_Text_QuickEnd()
        {
            var tokens = MarkupLinter.Lint("[bold]hello[-]", out var diagnostics);

            Assert.Empty(diagnostics);
            Assert.Equal(3, tokens.Length);

            EqualsKindAndValue(tokens[0], MarkupTokenKind.FormatStart, "bold");
            EqualsKindAndValue(tokens[1], MarkupTokenKind.Text, "hello");
            EqualsKindAndValue(tokens[2], MarkupTokenKind.FormatEnd, null);
        }

        [Fact]
        [Trait("Markup", "Lexer")]
        public void TwoFormatStart_Text_FormatEnd_Text_FormatEnd()
        {
            var tokens = MarkupLinter.Lint("[bold,italic]hello[-bold]world[-italic]", out var diagnostics);

            Assert.Empty(diagnostics);
            Assert.Equal(6, tokens.Length);

            EqualsKindAndValue(tokens[0], MarkupTokenKind.FormatStart, "bold");
            EqualsKindAndValue(tokens[1], MarkupTokenKind.FormatStart, "italic");
            EqualsKindAndValue(tokens[2], MarkupTokenKind.Text, "hello");
            EqualsKindAndValue(tokens[3], MarkupTokenKind.FormatEnd, "bold");
            EqualsKindAndValue(tokens[4], MarkupTokenKind.Text, "world");
            EqualsKindAndValue(tokens[5], MarkupTokenKind.FormatEnd, "italic");
        }

        [Fact]
        [Trait("Markup", "Lexer")]
        public void Format_Text_Whitespace_Text_Newline_FormatEnd()
        {
            var tokens = MarkupLinter.Lint("[bold]hello world\r\n[-bold]", out var diagnostics);

            Assert.Empty(diagnostics);
            Assert.Equal(6, tokens.Length);

            EqualsKindAndValue(tokens[0], MarkupTokenKind.FormatStart, "bold");
            EqualsKindAndValue(tokens[1], MarkupTokenKind.Text, "hello");
            EqualsKindAndValue(tokens[2], MarkupTokenKind.WhiteSpace, " ");
            EqualsKindAndValue(tokens[3], MarkupTokenKind.Text, "world");
            EqualsKindAndValue(tokens[4], MarkupTokenKind.NewLine);
            EqualsKindAndValue(tokens[5], MarkupTokenKind.FormatEnd, "bold");
        }

        [Fact]
        [Trait("Markup", "Lexer")]
        public void FormatToken_Text()
        {
            var tokens = MarkupLinter.Lint("[~br]hello", out var diagnostics);

            Assert.Empty(diagnostics);
            Assert.Equal(2, tokens.Length);

            EqualsKindAndValue(tokens[0], MarkupTokenKind.FormatToken, "br");
            EqualsKindAndValue(tokens[1], MarkupTokenKind.Text, "hello");
        }

        [Fact]
        [Trait("Markup", "Lexer")]
        public void FormatToken_WithVariable_Text()
        {
            var tokens = MarkupLinter.Lint("[~br:test]hello", out var diagnostics);

            Assert.Empty(diagnostics);
            Assert.Equal(3, tokens.Length);

            EqualsKindAndValue(tokens[0], MarkupTokenKind.FormatToken, "br");
            EqualsKindAndValue(tokens[1], MarkupTokenKind.FormatVariable, "test");
            EqualsKindAndValue(tokens[2], MarkupTokenKind.Text, "hello");
        }

        [Fact]
        [Trait("Markup", "Lexer")]
        public void FormatToken_WithVariable_Format_Text_FormatEnd()
        {
            var tokens = MarkupLinter.Lint("[~br:test,bold]hello[-bold]", out var diagnostics);

            Assert.Empty(diagnostics);
            Assert.Equal(5, tokens.Length);

            EqualsKindAndValue(tokens[0], MarkupTokenKind.FormatToken, "br");
            EqualsKindAndValue(tokens[1], MarkupTokenKind.FormatVariable, "test");
            EqualsKindAndValue(tokens[2], MarkupTokenKind.FormatStart, "bold");
            EqualsKindAndValue(tokens[3], MarkupTokenKind.Text, "hello");
            EqualsKindAndValue(tokens[4], MarkupTokenKind.FormatEnd, "bold");
        }

        [Fact]
        [Trait("Markup", "Diagnostics")]
        public void Missing_End_Tag()
        {
            var tokens = MarkupLinter.Lint("[bold]hello", out var diagnostics);

            Assert.Single(diagnostics);
            Assert.Equal(2, tokens.Length);

            EqualsKindAndValue(tokens[0], MarkupTokenKind.FormatStart, "bold");
            EqualsKindAndValue(tokens[1], MarkupTokenKind.Text, "hello");

            switch (diagnostics.First())
            {
                case MissingTag a when a.Position == MissingTag.TagPosition.End:
                    Assert.Equal("bold", a.Tag);
                    break;
                default:
                    Assert.True(false, "Missing tag should be at TagPosition End and with the tag <bold>");
                    break;
            }
        }


    }
}
