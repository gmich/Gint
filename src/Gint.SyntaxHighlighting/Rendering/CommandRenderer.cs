using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Gint;
using Gint.SyntaxHighlighting.Analysis;

namespace Gint.SyntaxHighlighting
{
    internal class TextSpanEqualityComparer : IEqualityComparer<TextSpan>
    {
        public bool Equals(TextSpan x, TextSpan y)
        {
            return (x.Start == y.Start) && (x.End == y.End);
        }

        public int GetHashCode([DisallowNull] TextSpan obj)
        {
            return obj.GetHashCode();
        }
    }

    internal class RenderItemEqualityComparer : IEqualityComparer<RenderItem>
    {
        public bool Equals(RenderItem x, RenderItem y)
        {
            return (x.Location.Start == y.Location.Start) && (x.Location.End == y.Location.End);
        }

        public int GetHashCode([DisallowNull] RenderItem obj)
        {
            return obj.GetHashCode();
        }
    }

    internal class CommandRenderer
    {
        private Suggestion suggestion = null;
        public int SuggestionLength => suggestion?.TotalSize ?? 0;

        private IEnumerable<int> errorPositions = null;
        private void _Render(string command, CommandRegistry registry)
        {
            if (string.IsNullOrEmpty(command)) return;

            testbuffer = string.Empty;
            //var boundNode = CommandBinder.Bind(command, registry, out var diagnostics);

            //var errorRenderItems = diagnostics.Select(c => new ErrorRenderItem(c.Location, c));
            //var boundRenderItems = BoundNodeRenderItemTraverser.GetRenderItems(boundNode);

            var expressionTree = CommandExpressionTree.Parse(command);
            errorPositions = expressionTree.Diagnostics.SelectMany(c => Enumerable.Range(c.Location.Start, c.Location.End)).Distinct();
            var expressionRenderitems = ExpressionRenderItemTraverser.GetRenderItems(expressionTree.Root);
            var highlightedRenderItems = SyntaxHighlighterLexer.Tokenize(command).Select(c => new HighlighterRenderItem(c));

            //var renderGroups = errorRenderItems.Concat(boundRenderItems).Concat(highlightedRenderItems).GroupBy(c => c.Location, new TextSpanEqualityComparer());
            var renderGroups = expressionRenderitems.Concat(highlightedRenderItems).GroupBy(c => c.Location, new TextSpanEqualityComparer());
            //var renderGroups = boundRenderItems.GroupBy(c => c.Location, new TextSpanEqualityComparer());

            var renderItems = new List<RenderItem>();
            foreach (var group in renderGroups)
            {
                var mostImportantOfGroup = group.OrderBy(c => (int)c.RenderItemType).First();
                renderItems.Add(mostImportantOfGroup);
            }
            var orderedRenderItems = renderItems.Distinct(new RenderItemEqualityComparer()).OrderBy(c => c.Location.Start);

            var test = expressionRenderitems.Concat(highlightedRenderItems).OrderBy(c => c.Location.Start);
            EvaluateRenderItems(test, command);
        }

        private void EvaluateRenderItems(IEnumerable<RenderItem> renderItems, string command)
        {
            RenderItem previous = null;
            foreach (var item in renderItems)
            {
                var location = item.Location;
                if (previous != null)
                {
                    if (location.Start < previous.Location.End)
                    {
                        var shift = previous.Location.End - location.Start;
                        location = new TextSpan(item.Location.Start + shift, item.Location.Length - shift);
                    }
                }
                switch (item.RenderItemType)
                {
                    case RenderItemType.Error:
                        RenderError(GetString(location, command));
                        break;
                    case RenderItemType.BoundNode:
                        RenderBoundToken(((BoundRenderItem)item).BoundNodeKind, GetString(item.Location, command));
                        break;
                    case RenderItemType.HighlighterLexer:
                        RenderHighlightToken(((HighlighterRenderItem)item).Token);
                        break;
                    case RenderItemType.ExpressionSyntax:
                        RenderExpressionToken(((ExpressionRenderItem)item).Kind, GetString(item.Location, command));
                        break;
                }
                previous = item;
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
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (text[i] == '\'' || text[i] == '\"')
                            RenderText(text[i].ToString(), ConsoleColor.Magenta);
                        else
                            RenderText(text[i].ToString(), ConsoleColor.White);
                    }
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

        private string GetString(TextSpan span, string text)
        {
            return text.Substring(span.Start, span.Length);
        }

        private void RenderError(string text)
        {
            RenderText(text, ConsoleColor.Red);
        }
        private void RenderBoundToken(BoundNodeKind kind, string text)
        {
            switch (kind)
            {
                case BoundNodeKind.Command:
                    RenderText(text, ConsoleColor.DarkGreen);
                    break;
                case BoundNodeKind.CommandWithVariable:
                    RenderText(text, ConsoleColor.DarkGreen);
                    break;
                case BoundNodeKind.Option:
                    RenderText(text, ConsoleColor.DarkYellow);
                    break;
                case BoundNodeKind.VariableOption:
                    RenderText(text, ConsoleColor.DarkYellow);
                    break;
                case BoundNodeKind.Pipe:
                    RenderText(text, ConsoleColor.DarkMagenta);
                    break;
                case BoundNodeKind.Pipeline:
                    break;
            }
        }

        private void RenderInternal(string text)
        {
            var tokens = SyntaxHighlighterLexer.Tokenize(text);

            foreach (var token in tokens)
            {
                RenderHighlightToken(token);
            }
        }

        private void RenderHighlightToken(HighlightToken token)
        {
            if (token.Kind == HighlightTokenKind.Whitespace)
            {
                if (testbuffer.Length > token.Span.Start) return;
                RenderWhiteSpace(token);
            }
            if (token.Kind == HighlightTokenKind.SingleQuote)
            {
                if (testbuffer.Length > token.Span.Start) return;
                RenderDoubleQuotes(token);
            }
            if (token.Kind == HighlightTokenKind.DoubleQuotes)
            {
                if (testbuffer.Length > token.Span.Start) return;
                RenderSingleQuote(token);
            }
            return;
            switch (token.Kind)
            {
                case HighlightTokenKind.Unknown:
                    RenderUnknown(token);
                    break;
                case HighlightTokenKind.Whitespace:
                    RenderWhiteSpace(token);
                    break;
                case HighlightTokenKind.Option:
                    RenderOption(token);
                    break;
                case HighlightTokenKind.Keyword:
                    RenderKeyword(token);
                    break;
                case HighlightTokenKind.EOF:
                    break;
                case HighlightTokenKind.Pipe:
                    RenderPipe(token);
                    break;
                case HighlightTokenKind.DoubleQuotes:
                    RenderDoubleQuotes(token);
                    break;
                case HighlightTokenKind.SingleQuote:
                    RenderSingleQuote(token);
                    break;
            }
        }

        private void RenderUnknown(HighlightToken token)
        {
            RenderText(token.Text, ConsoleColor.White);
        }

        private void RenderWhiteSpace(HighlightToken token)
        {
            RenderText(token.Text, ConsoleColor.White);
        }

        private void RenderOption(HighlightToken token)
        {
            RenderText(token.Text, ConsoleColor.Yellow);
        }

        private void RenderKeyword(HighlightToken token)
        {
            RenderText(token.Text, ConsoleColor.Green);
        }

        private void RenderPipe(HighlightToken token)
        {
            RenderText(token.Text, ConsoleColor.Magenta);
        }

        private void RenderDoubleQuotes(HighlightToken token)
        {
            RenderText(token.Text, ConsoleColor.Magenta);
        }

        private void RenderSingleQuote(HighlightToken token)
        {
            RenderText(token.Text, ConsoleColor.Magenta);
        }

        private string testbuffer = string.Empty;
        private void RenderText(string text, ConsoleColor color)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (errorPositions.Contains(testbuffer.Length))
                    Console.ForegroundColor = ConsoleColor.Red;
                else
                    Console.ForegroundColor = color;

                Console.Write(text[i]);
                testbuffer += text[i];
            }
            Console.ResetColor();
        }

        public void Render(string command)
        {
            _Render(command, CommandRegistry.Empty);
            //RenderInternal(command);
        }
    }


}