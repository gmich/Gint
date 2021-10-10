using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Gint;
using Gint.SyntaxHighlighting.Analysis;

namespace Gint.SyntaxHighlighting
{

    internal class CommandRenderer
    {
        private IEnumerable<int> errorCells;
        private string textProcessed;
        private string textRendered;
        private Action<ConsoleColor> cellColorer;
        private Action renderCallback;
        private BoundNode boundNode;
        private CommandExpressionTree expressionTree;

        public CommandRenderer()
        {
            cellColorer = PlainCellColorer;
            Suggestions = new SuggestionRenderer();
            Reset();
        }

        public SuggestionRenderer Suggestions { get; }

        public CommandRegistry Registry { get; init; }

        private void Noop() { }

        private void Reset()
        {
            renderCallback = Noop;
            errorCells = Enumerable.Empty<int>();
            textProcessed = string.Empty;
            textRendered = string.Empty;
        }

        private void SetErrorCells(DiagnosticCollection diagnostics)
        {
            errorCells = diagnostics
                .SelectMany(c => Enumerable.Range(c.Location.Start, c.Location.End))
                .Distinct();
        }


        private bool _displayErrorCells = false;
        public bool DisplayErrorCells
        {
            get
            {
                return _displayErrorCells;
            }
            set
            {
                _displayErrorCells = value;
                cellColorer = _displayErrorCells ? ErrorAwareCellColorer : PlainCellColorer;
            }
        }

        public bool DisplayDiagnostics { get; set; } = false;

        private void ErrorAwareCellColorer(ConsoleColor color)
        {
            if (errorCells.Contains(textRendered.Length))
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = color;
        }

        private void PlainCellColorer(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        private void RenderInternal(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                renderCallback = Noop;
                RenderSuggestions();
                if (DisplayDiagnostics)
                    RenderDiagnosticsFrame();

                return;
            }

            Reset();

            expressionTree = CommandExpressionTree.Parse(command);

            if (DisplayErrorCells)
                SetErrorCells(expressionTree.Diagnostics);

            var binder = new CommandBinder(expressionTree.Root, Registry);
            try
            {
                boundNode = binder.Bind();
            }
            catch { }
            expressionTree.Diagnostics.AddRange(binder.Diagnostics);

            var expressionRenderitems = ExpressionRenderItemTraverser.GetRenderItems(expressionTree.Root);
            var highlightedRenderItems = SyntaxHighlighterLexer.Tokenize(command).Select(c => new HighlighterRenderItem(c));

            var renderItems = expressionRenderitems.Concat(highlightedRenderItems).OrderBy(c => c.Location.Start);

            EvaluateRenderItems(renderItems, command);

            RenderSuggestions();

            RenderDiagnostics(expressionTree.Diagnostics, command);
        }

        public void RenderSuggestions()
        {
            if (Suggestions.HasFocus)
            {
                renderCallback += Suggestions.GenerateRenderCallback();
            }
        }

        public void DisplaySuggestions()
        {
            SuggestionsEngine.DisplaySuggestions(textProcessed, Suggestions, expressionTree, boundNode, Registry);
        }

        private void RenderDiagnosticsFrame()
        {
            renderCallback += () =>
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                var diagnosticsText = "--Diagnostics";
                Console.Write(diagnosticsText);
                Console.WriteLine(new string('—', Console.BufferWidth - diagnosticsText.Length));
                Console.ResetColor();
                Console.WriteLine();
            };
        }

        private bool ShouldPrintDiagnostic(Diagnostic diagnostic)
        {
            return (diagnostic.ErrorCode != DiagnosticsErrorCode.NullCommand
             && diagnostic.ErrorCode != DiagnosticsErrorCode.NullOption);
        }

        private void RenderDiagnostics(DiagnosticCollection diagnostics, string text)
        {
            if (!DisplayDiagnostics) return;

            RenderDiagnosticsFrame();

            renderCallback += () =>
            {
                for (int i = 0; i < diagnostics.Count(); i++)
                {
                    var diagnostic = diagnostics.ElementAt(i);
                    if (!ShouldPrintDiagnostic(diagnostic)) continue;

                    var error = text.Substring(diagnostic.Location.Start, diagnostic.Location.Length);
                    var prefix = text.Substring(0, diagnostic.Location.Start);
                    var suffix = text[diagnostic.Location.End..];
                    Console.Write("    ");
                    Console.Write(prefix);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(error);
                    Console.ResetColor();
                    Console.WriteLine(suffix);

                    if (diagnostic.IsError)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" X ");
                        Console.ResetColor();
                        Console.Write(" ");
                    }
                    else //warning
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(" ! ");
                        Console.ResetColor();
                        Console.Write(" ");
                    }
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(diagnostic.ErrorCode);
                    Console.ResetColor();
                    Console.Write(" ");
                    Console.WriteLine(diagnostic.Message);
                    Console.WriteLine();
                }
            };
        }

        private void EvaluateRenderItems(IEnumerable<RenderItem> renderItems, string command)
        {
            foreach (var item in renderItems)
            {
                switch (item.RenderItemType)
                {
                    case RenderItemType.HighlighterLexer:
                        RenderHighlightToken(((HighlighterRenderItem)item).Token);
                        break;
                    case RenderItemType.ExpressionSyntax:
                        RenderExpressionToken(((ExpressionRenderItem)item).Kind, GetStringFromSpan(item.Location, command));
                        break;
                }
            }
        }

        private void RenderExpressionToken(CommandTokenKind kind, string text)
        {
            switch (kind)
            {
                case CommandTokenKind.CommandExpression:
                    RenderText(text, ConsoleColor.Green);
                    break;
                case CommandTokenKind.CommandWithVariableExpression:
                    RenderText(text, ConsoleColor.Green);
                    break;
                case CommandTokenKind.Keyword:
                    RenderKeyword(text);
                    break;
                case CommandTokenKind.OptionExpression:
                    RenderText(text, ConsoleColor.Yellow);
                    break;
                case CommandTokenKind.VariableOptionExpression:
                    RenderText(text, ConsoleColor.Yellow);
                    break;
                case CommandTokenKind.PipeExpression:
                    RenderText(text, ConsoleColor.Magenta);
                    break;
                case CommandTokenKind.PipedCommandExpression:
                    RenderText(text, ConsoleColor.Magenta);
                    break;
            }
        }

        private void RenderKeyword(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\'' || text[i] == '\"')
                    RenderText(text[i].ToString(), ConsoleColor.Magenta);
                else
                    RenderText(text[i].ToString(), ConsoleColor.White);
            }
        }

        private static string GetStringFromSpan(TextSpan span, string text)
        {
            return text.Substring(span.Start, span.Length);
        }

        private void RenderHighlightToken(HighlightToken token)
        {
            if (token.Kind == HighlightTokenKind.Whitespace)
            {
                if (textProcessed.Length > token.Span.Start) return;

                RenderText(token.Text, ConsoleColor.White);
            }
        }

        private void RenderText(string text, ConsoleColor color)
        {
            textProcessed += text;
            renderCallback += () => RenderCell(text, color);
        }

        private void RenderCell(string text, ConsoleColor color)
        {
            for (int i = 0; i < text.Length; i++)
            {
                cellColorer(color);
                Console.Write(text[i]);
                textRendered += text[i];
            }
            Console.ResetColor();
        }

        public Action GenerateRenderCallback(string command)
        {
            RenderInternal(command);
            return renderCallback;
        }

    }


}