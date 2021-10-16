using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Gint;
using Gint.Terminal.Analysis;

namespace Gint.Terminal
{

    internal class CommandRenderer
    {
        private readonly TerminalOptions options;

        private IEnumerable<int> errorCells;
        private string textProcessed;
        private string textRendered;
        private Action<ThemeColor> cellColorer;
        private Action renderCallback;
        private BoundNode boundNode;
        private CommandExpressionTree expressionTree;
        public SuggestionEngine SuggestionEngine { get; }

        public CommandRenderer(TerminalOptions options)
        {
            this.options = options;
            Registry = options.Registry;
            cellColorer = CellColorer;
            SuggestionEngine = new SuggestionEngine(options);
            DisplayErrorCells = options.DisplayErrorCells;
            DisplayDiagnostics = options.DisplayDiagnostics;
            Reset();
        }

        public CommandRegistry Registry { get; }

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
                cellColorer = _displayErrorCells ? ErrorAwareCellColorer : CellColorer;
            }
        }

        public bool DisplayDiagnostics { get; set; } = false;

        private void ErrorAwareCellColorer(ThemeColor color)
        {
            if (errorCells.Contains(textRendered.Length))
            {
                options.Theme.ErrorCell.Apply();
            }
            else
            {
                color.Apply();
            }
        }

        private void CellColorer(ThemeColor color)
        {
            color.Apply();
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

            CompileCommand(command);

            if (DisplayErrorCells)
                SetErrorCells(expressionTree.Diagnostics);

            var expressionRenderitems = ExpressionRenderItemTraverser.GetRenderItems(expressionTree.Root);
            var highlightedRenderItems = SyntaxHighlighterLexer.Tokenize(command).Select(c => new HighlighterRenderItem(c));

            var renderItems = expressionRenderitems.Concat(highlightedRenderItems).OrderBy(c => c.Location.Start);

            EvaluateRenderItems(renderItems, command);

            RenderSuggestions();

            RenderDiagnostics(expressionTree.Diagnostics, command);
        }

        private void CompileCommand(string command)
        {
            expressionTree = CommandExpressionTree.Parse(command);
            var binder = new CommandBinder(expressionTree.Root, Registry);
            try
            {
                boundNode = binder.Bind();
            }
            catch { }

            expressionTree.Diagnostics.AddRange(binder.Diagnostics);
        }

        public void RenderSuggestions()
        {
            if (SuggestionEngine.InputHandler.HasFocus)
            {
                renderCallback += SuggestionEngine.GenerateRenderCallback();
            }
        }

        public void DisplaySuggestions(string command)
        {
            if(command != textProcessed)
                CompileCommand(command);

            SuggestionEngine.Run(command, expressionTree, boundNode);
        }

        private void RenderDiagnosticsFrame()
        {
            renderCallback += () =>
            {
                Console.WriteLine();
                Console.WriteLine();
                cellColorer(options.Theme.DiagnosticsFrame);
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
                    options.Theme.ErrorCell.Apply();
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
                    options.Theme.DiagnosticsCode.Apply();
                    Console.Write(diagnostic.ErrorCode);
                    Console.ResetColor();
                    options.Theme.DiagnosticsMessage.Apply();
                    Console.Write(" ");
                    Console.Write(diagnostic.Message);
                    Console.ResetColor();
                    Console.WriteLine();
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
                    RenderText(text, options.Theme.Command);
                    break;
                case CommandTokenKind.CommandWithVariableExpression:
                    RenderText(text, options.Theme.CommandWithVariable);
                    break;
                case CommandTokenKind.Keyword:
                    RenderKeyword(text);
                    break;
                case CommandTokenKind.OptionExpression:
                    RenderText(text, options.Theme.Option);
                    break;
                case CommandTokenKind.VariableOptionExpression:
                    RenderText(text, options.Theme.OptionWithVariable);
                    break;
                case CommandTokenKind.PipeExpression:
                    RenderText(text, options.Theme.Pipe);
                    break;
                case CommandTokenKind.PipedCommandExpression:
                    RenderText(text, options.Theme.Pipe);
                    break;
            }
        }

        private void RenderKeyword(string text)
        {
            bool isFirstOrLastCharacter(int index) => index == 0 || index == text.Length - 1;

            for (int i = 0; i < text.Length; i++)
            {
                if (isFirstOrLastCharacter(i) && (text[i] == '\'' || text[i] == '\"'))
                    RenderText(text[i].ToString(), options.Theme.Quotes);
                else
                    RenderText(text[i].ToString(), options.Theme.Keyword);
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

                RenderText(token.Text, options.Theme.Whitespace);
            }
        }

        private void RenderText(string text, ThemeColor color)
        {
            textProcessed += text;
            renderCallback += () => RenderCell(text, color);
        }

        private void RenderCell(string text, ThemeColor color)
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