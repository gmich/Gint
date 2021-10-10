using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gint.SyntaxHighlighting
{
    internal static class SuggestionModelCallbackExtensions
    {
        public static SuggestionModelCallback ToKeywordSuggestions(this SuggestionsCallback callback)
        {
            return var => callback(var).Select(c => new SuggestionModel(c.DisplayValue, c.Value, SuggestionType.Keyword));
        }
        public static IEnumerable<SuggestionModel> EmptySuggestions(string var) => Enumerable.Empty<SuggestionModel>();


        public static SuggestionModelCallback With(this SuggestionModelCallback callback, SuggestionModelCallback callbackup2)
        {
            return (var) => callback(var).Concat(callbackup2(var));
        }

    }

    internal delegate IEnumerable<SuggestionModel> SuggestionModelCallback(string variable);

    internal class SuggestionsEngine
    {
        public static void DisplaySuggestions(string command, SuggestionRenderer renderer, CommandExpressionTree expressionTree, BoundNode boundNode, CommandRegistry registry)
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
            Render(renderer, suggestionsCallback, variable);
        }

        private static void Render(SuggestionRenderer renderer, SuggestionModelCallback suggestionsCallback, string variable)
        {
            var suggestions = suggestionsCallback(variable).ToArray();
            if (suggestions.Length > 0)
            {
                renderer.Init(suggestions);
            }
        }

        private static SuggestionModelCallback VariableOptionSuggestion(CommandRegistry registry, Stack<BoundNode> nodeStack, out string variable, BoundNode lastNode)
        {
            var node = (BoundVariableOption)lastNode;
            variable = node.Variable;
            //var suggestionsCallback = node.VariableOption.Suggestions.ToKeywordSuggestions();
            SuggestionModelCallback suggestionsCallback = SuggestionModelCallbackExtensions.EmptySuggestions;

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

            if (registry.Collection.ContainsKey(cmd))
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
            var command = (CommandWithVariable)node.Command;

            var suggestionsCallback = node.Command.Suggestions.ToKeywordSuggestions();
            if (command.Required)
            {
                return suggestionsCallback;
            }

            return suggestionsCallback.With(SuggestCommandOptions(registry, node));
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
