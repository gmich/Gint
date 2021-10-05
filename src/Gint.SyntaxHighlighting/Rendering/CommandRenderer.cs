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
        private Suggestion suggestion;
        public int SuggestionLength => suggestion?.TotalSize ?? 0;
        private IEnumerable<int> errorCells;
        private string textRendered;
        private Action<ConsoleColor> cellColorer;
        private Action renderCallback;

        public CommandRenderer()
        {
            cellColorer = PlainCellColorer;
            Reset();
        }

        public CommandRegistry Registry { get; init; }

        private void Noop() { }

        private void Reset()
        {
            renderCallback = Noop;
            suggestion = null;
            errorCells = Enumerable.Empty<int>();
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
            Reset();
            var expressionTree = CommandExpressionTree.Parse(command);

            if (DisplayErrorCells)
                SetErrorCells(expressionTree.Diagnostics);

            var expressionRenderitems = ExpressionRenderItemTraverser.GetRenderItems(expressionTree.Root);
            var highlightedRenderItems = SyntaxHighlighterLexer.Tokenize(command).Select(c => new HighlighterRenderItem(c));

            var renderItems = expressionRenderitems.Concat(highlightedRenderItems).OrderBy(c => c.Location.Start);

            EvaluateRenderItems(renderItems, command);
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
                    RenderText(text[i].ToString(), ConsoleColor.Gray);
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
                if (textRendered.Length > token.Span.Start) return;

                RenderText(token.Text, ConsoleColor.White);
            }
        }

        private void RenderText(string text, ConsoleColor color)
        {
            textRendered += text;
            renderCallback += () => RenderCell(text, color);
        }

        private void RenderCell(string text, ConsoleColor color)
        {
            for (int i = 0; i < text.Length; i++)
            {
                cellColorer(color);
                Console.Write(text[i]);         
            }
            Console.ResetColor();
        }

        public Action GenerateRenderCallback(string command)
        {
            if (string.IsNullOrEmpty(command)) return Noop;

            RenderInternal(command);
            return renderCallback;
        }
    }


}