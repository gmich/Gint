using System;
using System.Collections.Generic;
using System.Text;

namespace Gint
{
    internal class CommandTokenizer
    {
        public static IEnumerable<CommandSyntaxToken> Tokenize(string text, out DiagnosticCollection diagnostics)
        {
            var lexer = new CommandLexer(text);
            diagnostics = lexer.Diagnostics;
            var tokens = new List<CommandSyntaxToken>();

            while (true)
            {
                var token = lexer.Lex();
                if (token.Kind == CommandTokenKind.End)
                    break;

                tokens.Add(token);

            }
            return tokens;
        }
    }
}
