using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gint.Terminal
{
    internal delegate IEnumerable<SuggestionModel> SuggestionModelCallback(string variable);

    internal class SuggestionEngine
    {
        private readonly TerminalOptions options;
        private readonly CommandRegistry registry;
        internal SuggestionInputHandler InputHandler { get; }

        public SuggestionEngine(TerminalOptions options)
        {
            this.options = options;
            this.registry = options.Registry;
            InputHandler = new SuggestionInputHandler();
        }

        public void Run(string command, CommandExpressionTree expressionTree, BoundNode boundNode)
        {
            SuggestionModelCallback suggestionsCallback = SuggestionModelCallbackExtensions.EmptySuggestions;
            string variable = null;

            if (expressionTree != null && boundNode != null)
            {
                var nodeStack = BoundNodeStackConveter.GetBoundNodeStack(boundNode);
                var lastNode = nodeStack.Pop();

                switch (lastNode.Kind)
                {
                    case BoundNodeKind.Command:
                        suggestionsCallback = CommandSuggestion(registry, lastNode);
                        break;
                    case BoundNodeKind.CommandWithVariable:
                        suggestionsCallback = CommandWithVariableSuggestion(registry, out variable, lastNode);
                        break;
                    case BoundNodeKind.Option:
                        suggestionsCallback = OptionSuggestion(registry, nodeStack, lastNode);
                        break;
                    case BoundNodeKind.VariableOption:
                        suggestionsCallback = VariableOptionSuggestion(registry, nodeStack, out variable, lastNode);
                        break;
                }
            }
            else
            {
                suggestionsCallback= SuggestAutocompleteCommands(registry, null);
            }
            InternalRun(suggestionsCallback, variable);
        }

        public Action GenerateRenderCallback() => InputHandler.GenerateRenderCallback();

        private void InternalRun(SuggestionModelCallback suggestionsCallback, string variable)
        {
            var suggestions = suggestionsCallback(variable).ToArray();
            if (suggestions.Length > 0)
            {
                InputHandler.Handle(new SuggestionRenderer(suggestions, options.MaxSuggestionsPerRow, options.Theme));
            }
        }

        private static SuggestionModelCallback VariableOptionSuggestion(CommandRegistry registry, Stack<BoundNode> nodeStack, out string variable, BoundNode lastNode)
        {
            var node = (BoundVariableOption)lastNode;
            variable = node.Variable;
            SuggestionModelCallback suggestionsCallback = node.VariableOption.Suggestions.ToAutoCompleteSuggestions();

            IterateStackForBoundCommands(nodeStack,
            onBoundCommandFound: c =>
            {
                suggestionsCallback = suggestionsCallback.With(SuggestCommandOptions(registry, c));
            },
            onBoundCommandWithVariableFound: d =>
            {
                suggestionsCallback = suggestionsCallback.With(SuggestCommandOptions(registry, d));
            });

            return suggestionsCallback;
        }

        private static SuggestionModelCallback OptionSuggestion(CommandRegistry registry, Stack<BoundNode> nodeStack, BoundNode lastNode)
        {
            var node = (BoundOption)lastNode;
            var suggestionsCallback = node.Option.Suggestions.ToKeywordSuggestions();
            var option = node.Option;

            IterateStackForBoundCommands(nodeStack,
                onBoundCommandFound: c =>
                {
                    suggestionsCallback = suggestionsCallback.With(SuggestOptions(registry, c, option));
                },
                onBoundCommandWithVariableFound: d =>
                {
                    suggestionsCallback = suggestionsCallback.With(SuggestOptions(registry, d, option));
                });

            return suggestionsCallback;
        }

        private static void IterateStackForBoundCommands(Stack<BoundNode> nodeStack, Action<BoundCommand> onBoundCommandFound, Action<BoundCommandWithVariable> onBoundCommandWithVariableFound)
        {
            while (nodeStack.Count > 0)
            {
                var previousNode = nodeStack.Pop();
                if (previousNode is BoundCommand c)
                {
                    onBoundCommandFound(c);
                    break;
                }
                else if (previousNode is BoundCommandWithVariable d)
                {
                    onBoundCommandWithVariableFound(d);
                    break;
                }
            }
        }

        private static SuggestionModelCallback SuggestOptions(CommandRegistry registry, BoundCommand node, Option option)
        {
            SuggestionModelCallback alternative = SuggestionModelCallbackExtensions.EmptySuggestions;
            var cmd = node.Command.CommandName;

            if (cmd != null && registry.Collection.ContainsKey(cmd))
            {
                var options = registry
                    .Collection
                    .Where(c => c.Key == node.Command.CommandName)
                    .First()
                    .Value.Options;

                var optionFromRegistry = options.Where(c => c.LongArgument == option.LongArgument).FirstOrDefault();
                if (optionFromRegistry != null)
                {
                    if (optionFromRegistry is VariableOption vo)
                        alternative = vo.Suggestions.ToKeywordSuggestions();
                    else
                        alternative = (var) => options.Select(c => new SuggestionModel(c.LongArgument, c.LongArgument, SuggestionType.Keyword));
                }
                else
                {
                    alternative = (var) => options
                        .Select(c => c.LongArgument)
                        .Where(c => c.StartsWith(option.LongArgument))
                        .Select(c => new SuggestionModel(c, c.Remove(0, option.LongArgument.Length), SuggestionType.Autocomplete));
                }
            }

            return alternative;
        }

        private static SuggestionModelCallback CommandWithVariableSuggestion(CommandRegistry registry, out string variable, BoundNode lastNode)
        {
            var node = (BoundCommandWithVariable)lastNode;
            variable = node.Variable;

            if (node.Command is CommandWithVariable command)
            {
                var suggestions = node.Command.Suggestions;

                var suggestionsCallback = string.IsNullOrEmpty(variable) ? suggestions.ToKeywordSuggestions() : suggestions.ToAutoCompleteSuggestions();
                if (command.Required)
                {
                    return suggestionsCallback;
                }

                return suggestionsCallback.With(SuggestCommandOptions(registry, node));
            }
            else
            {
                return SuggestionModelCallbackExtensions.EmptySuggestions;
            }
        }

        private static SuggestionModelCallback CommandSuggestion(CommandRegistry registry, BoundNode lastNode)
        {
            var node = (BoundCommand)lastNode;
            var suggestionsCallback = node.Command.Suggestions.ToKeywordSuggestions();

            return suggestionsCallback.With(SuggestCommandOptions(registry, node));
        }

        private static SuggestionModelCallback SuggestCommandOptions(CommandRegistry registry, BoundCommand node)
        {
            SuggestionModelCallback alternative;
            var cmd = node.Command.CommandName;

            if (cmd != null && registry.Collection.ContainsKey(cmd))
                alternative = (var) => registry
                    .Collection
                    .Where(c => c.Key == node.Command.CommandName)
                    .First()
                    .Value.Options.Select(c => c.LongArgument)
                    .Select(c => new SuggestionModel(c, c, SuggestionType.Keyword));
            else
                alternative = SuggestAutocompleteCommands(registry, node);

            return alternative;
        }

        private static SuggestionModelCallback SuggestAutocompleteCommands(CommandRegistry registry, BoundCommand node)
        {
            SuggestionModelCallback alternative;
            var cmd = node?.Command.CommandName;

            if (cmd == null)
            {
                alternative = (var) => registry
                .Collection
                .Select(c => new SuggestionModel(c.Key, c.Key, SuggestionType.Keyword));
            }
            else
            {
                alternative = (var) => registry
                    .Collection
                    .Where(c => c.Key.StartsWith(node.Command.CommandName))
                    .Select(c => new SuggestionModel(c.Key, c.Key.Remove(0, cmd.Length), SuggestionType.Autocomplete));
            }
            return alternative;
        }

    }
}
