using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gint
{

    internal class Parser
    {
        private readonly CommandSyntaxToken[] commandTokens;
        private readonly DiagnosticCollection diagnostics = new DiagnosticCollection();
        private int position;
        private readonly string text;

        public Parser(string text)
        {
            this.text = text;
            var commandTokens = new List<CommandSyntaxToken>();

            var lexer = new CommandLexer(text);
            CommandSyntaxToken token;
            do
            {
                token = lexer.Lex();

                if (token.Kind != CommandTokenKind.WhiteSpace)
                {
                    commandTokens.Add(token);
                }
            } while (token.Kind != CommandTokenKind.End);

            this.commandTokens = commandTokens.ToArray();
            diagnostics.AddRange(lexer.Diagnostics);
        }

        public DiagnosticCollection Diagnostics => diagnostics;

        private CommandSyntaxToken Peek(int offset)
        {
            var index = position + offset;
            if (index >= commandTokens.Length)
                return commandTokens[commandTokens.Length - 1];

            return commandTokens[index];
        }

        private CommandSyntaxToken Current => Peek(0);

        private CommandSyntaxToken NextToken()
        {
            var current = Current;
            position++;
            return current;
        }

        private CommandSyntaxToken MatchToken(CommandTokenKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, kind);
            return new CommandSyntaxToken(kind, Current.Position, null, null);
        }


        public CommandExpressionTree Parse()
        {
            var expresion = ParseExecutionPipeline();
            var endOfFileToken = MatchToken(CommandTokenKind.End);
            return new CommandExpressionTree(diagnostics, expresion, endOfFileToken, text);
        }

        private ExpressionSyntax ParseExecutionPipeline()
        {
            CommandExpressionSyntax commandExpressionSyntax = null;
            while (Current.Kind != CommandTokenKind.End &&
                  Current.Kind != CommandTokenKind.Pipe)
            {
                commandExpressionSyntax = ParseCommand();
            }
            if (Current.Kind == CommandTokenKind.Pipe)
            {
                var pipe = ParsePipe();
                var second = ParseExecutionPipeline();
                if(commandExpressionSyntax==null)
                {
                    diagnostics.ReportMissingCommandToPipe(pipe.Span);
                }
                return new PipedCommandExpressionSyntax(commandExpressionSyntax, pipe, second);
            }
            if (commandExpressionSyntax == null)
            {
                diagnostics.ReportUnterminatedPipeline(Current.Span);
            }
            //TODO: dont allow null expression syntax
            return commandExpressionSyntax;
        }

        private CommandExpressionSyntax ParseCommand()
        {
            List<ExpressionSyntax> options = new List<ExpressionSyntax>();
            var commandToken = MatchToken(CommandTokenKind.Keyword);

            CommandSyntaxToken commandVariableToken = null;
            if (Peek(0).Kind == CommandTokenKind.Keyword)
                commandVariableToken = MatchToken(CommandTokenKind.Keyword);

            while (Current.Kind != CommandTokenKind.End &&
                  Current.Kind != CommandTokenKind.Pipe)
            {
                var option = ParseOption();
                options.Add(option);
            }
            if (commandVariableToken != null)
            {
                return new CommandWithVariableExpressionSyntax(commandToken, commandVariableToken, options.ToArray());
            }
            return new CommandExpressionSyntax(commandToken, options.ToArray());
        }

        private PipeExpressionSyntax ParsePipe()
        {
            var token = MatchToken(CommandTokenKind.Pipe);
            return new PipeExpressionSyntax(token);
        }


        private ExpressionSyntax ParseOption()
        {
            var option = MatchToken(CommandTokenKind.Option);
            if (Current.Kind == CommandTokenKind.Keyword)
            {
                return new VariableOptionExpressionSyntax(option, NextToken());
            }
            else
            {
                return new OptionExpressionSyntax(option);
            }
        }
    }
}
