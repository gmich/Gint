using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;

namespace Gint.Tests
{
    public class CommandLexerTests
    {
        [Fact]
        public void Lexer_Lexes_Command()
        {
            var tokens = CommandTokenizer.Tokenize("hello --world", out var diagnostics);

            Assert.Empty(diagnostics);
            Assert.Equal(3, tokens.Count());
            Assert.Equal(CommandTokenKind.Keyword, tokens.First().Kind);
            Assert.Equal(CommandTokenKind.WhiteSpace, tokens.Skip(1).Take(1).First().Kind);
            Assert.Equal(CommandTokenKind.Option, tokens.Skip(2).Take(2).First().Kind);
        }

        [Fact]
        public void Lexer_Lexes_EscapedQuote()
        {
            var tokens = CommandTokenizer.Tokenize("\"hello \"\" world\"", out var diagnostics);

            Assert.Empty(diagnostics);
            Assert.Single(tokens);
            Assert.Equal(CommandTokenKind.Keyword, tokens.Single().Kind);
            Assert.Equal("hello \" world", tokens.Single().Value);
        }

        [Theory]
        [InlineData("hello", CommandTokenKind.Keyword, "hello")]
        [InlineData("-opt", CommandTokenKind.Option, "-opt")]
        [InlineData("--option", CommandTokenKind.Option, "--option")]
        [InlineData("\"hello world\"", CommandTokenKind.Keyword, "hello world")]
        [InlineData("   ", CommandTokenKind.WhiteSpace, "   ")]
        [InlineData(">", CommandTokenKind.Pipe, ">")]
        public void CommandLexer_Lexers_Indidividual_Tokens_Correctly(string command, CommandTokenKind token, string parsed)
        {
            var tokens = CommandTokenizer.Tokenize(command, out var diagnostics);

            Assert.Empty(diagnostics);
            Assert.Single(tokens);
            Assert.Equal(token, tokens.Single().Kind);
            Assert.Equal(parsed, tokens.Single().Value);
        }

        [Theory]
        [MemberData(nameof(GetLexerData))]
        public void Lexer_Lexes_Complex_Command(string command, TokenAndValue[] tokensAndValues)
        {
            var parsedTokens = CommandTokenizer.Tokenize(command, out var diagnostics).ToArray();

            Assert.Empty(diagnostics);
            Assert.Equal(tokensAndValues.Length, parsedTokens.Length);

            for (int i = 0; i < tokensAndValues.Length; i++)
            {
                Assert.Equal(tokensAndValues[i].Kind, parsedTokens[i].Kind);
                Assert.Equal(tokensAndValues[i].Value, parsedTokens[i].Value);
            }
        }

        public class TokenAndValue
        {
            public TokenAndValue(CommandTokenKind token, string value)
            {
                Value = value;
                Kind = token;
            }

            public string Value { get; }
            public CommandTokenKind Kind { get; }
        }

        public static IEnumerable<object[]> GetLexerData()
        {
            return new List<object[]>
            {
                new object[] { "test", new[] 
                    { 
                        new TokenAndValue(CommandTokenKind.Keyword, "test") 
                    }
                },
                new object[] { "h -w -ww --www > test \"hello \"\" world\"", new[]
                    {
                         new TokenAndValue(CommandTokenKind.Keyword, "h"),
                         new TokenAndValue(CommandTokenKind.WhiteSpace, " "),
                         new TokenAndValue(CommandTokenKind.Option, "-w"),
                         new TokenAndValue(CommandTokenKind.WhiteSpace, " "),
                         new TokenAndValue(CommandTokenKind.Option, "-ww"),
                         new TokenAndValue(CommandTokenKind.WhiteSpace, " "),
                         new TokenAndValue(CommandTokenKind.Option, "--www"),
                         new TokenAndValue(CommandTokenKind.WhiteSpace, " "),
                         new TokenAndValue(CommandTokenKind.Pipe, ">"),
                         new TokenAndValue(CommandTokenKind.WhiteSpace, " "),
                         new TokenAndValue(CommandTokenKind.Keyword, "test"),
                         new TokenAndValue(CommandTokenKind.WhiteSpace, " "),
                         new TokenAndValue(CommandTokenKind.Keyword, "hello \" world"),
                    }
                }
            };
        }
    }
}
