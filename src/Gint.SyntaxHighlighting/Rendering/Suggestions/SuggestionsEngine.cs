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

    }

    internal delegate IEnumerable<SuggestionModel> SuggestionModelCallback(string variable);

    internal class SuggestionsEngine
    {

        public static void DisplaySuggestions(string command, SuggestionRenderer renderer, CommandExpressionTree expressionTree, BoundNode boundNode, CommandRegistry registry)
        {
            SuggestionModelCallback suggestionsCallback = SuggestionModelCallbackExtensions.EmptySuggestions;
            SuggestionModelCallback alternative = SuggestionModelCallbackExtensions.EmptySuggestions;
            string variable = null;
            if (expressionTree != null && boundNode != null)
            {
                var nodeStack = BoundNodeStackConveter.GetBoundNodeStack(boundNode);
                var lastNode = nodeStack.Pop();

                switch (lastNode.Kind)
                {
                    case BoundNodeKind.Command:
                        CommandSuggestion(registry, out suggestionsCallback, out alternative, lastNode);
                        break;
                    case BoundNodeKind.CommandWithVariable:
                        CommandWithVariableSuggestion(registry, out suggestionsCallback, out alternative, out variable, lastNode);
                        break;
                    case BoundNodeKind.Option:
                        suggestionsCallback = OptionSuggestion(lastNode);
                        break;
                    case BoundNodeKind.VariableOption:
                        VariableOptionSuggestion(out suggestionsCallback, out variable, lastNode);
                        break;
                }
            }

            Render(renderer, suggestionsCallback, alternative, variable);
        }

        private static void Render(SuggestionRenderer renderer, SuggestionModelCallback suggestionsCallback, SuggestionModelCallback alternative, string variable)
        {
            var suggestions = suggestionsCallback(variable).ToArray();
            if (suggestions.Length > 0)
            {
                renderer.Init(suggestions);
            }
            else
            {
                var alternativeSugggestions = alternative(variable).ToArray();
                if (alternativeSugggestions.Length > 0)
                {
                    renderer.Init(alternativeSugggestions);
                }
            }
        }

        private static void VariableOptionSuggestion(out SuggestionModelCallback suggestionsCallback, out string variable, BoundNode lastNode)
        {
            var node3 = (BoundVariableOption)lastNode;
            variable = node3.Variable;
            suggestionsCallback = node3.VariableOption.Suggestions.ToKeywordSuggestions();
        }

        private static SuggestionModelCallback OptionSuggestion(BoundNode lastNode)
        {
            SuggestionModelCallback suggestionsCallback;
            var node2 = (BoundOption)lastNode;
            suggestionsCallback = node2.Option.Suggestions.ToKeywordSuggestions();
            return suggestionsCallback;
        }

        private static void CommandWithVariableSuggestion(CommandRegistry registry, out SuggestionModelCallback suggestionsCallback, out SuggestionModelCallback alternative, out string variable, BoundNode lastNode)
        {
            var node = (BoundCommandWithVariable)lastNode;
            variable = node.Variable;
            suggestionsCallback = node.Command.Suggestions.ToKeywordSuggestions();

            alternative = SuggestCommandOptions(registry, node);
        }

        private static void CommandSuggestion(CommandRegistry registry, out SuggestionModelCallback suggestionsCallback, out SuggestionModelCallback alternative, BoundNode lastNode)
        {
            var node = (BoundCommand)lastNode;
            suggestionsCallback = node.Command.Suggestions.ToKeywordSuggestions();

            alternative = SuggestCommandOptions(registry, node);
        }

        private static SuggestionModelCallback SuggestCommandOptions(CommandRegistry registry, BoundCommand node)
        {
            SuggestionModelCallback alternative;
            var cmd = node.Command.CommandName;

            if (registry.Collection.ContainsKey(cmd))
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
            var cmd = node.Command.CommandName;

            alternative = (var) => registry
                .Collection
                .Where(c => c.Key.StartsWith(node.Command.CommandName))
                .Select(c => new SuggestionModel(c.Key, c.Key.Remove(0, cmd.Length), SuggestionType.Autocomplete));

            return alternative;
        }

    }
}
