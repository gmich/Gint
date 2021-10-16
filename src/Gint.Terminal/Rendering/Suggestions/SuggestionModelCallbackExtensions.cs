using System.Collections.Generic;
using System.Linq;

namespace Gint.Terminal
{
    internal static class SuggestionModelCallbackExtensions
    {
        public static SuggestionModelCallback ToKeywordSuggestions(this SuggestionsCallback callback)
        {
            return var => callback(var).Select(c => new SuggestionModel(c.DisplayValue, c.Value, SuggestionType.Keyword));
        }

        public static SuggestionModelCallback ToAutoCompleteSuggestions(this SuggestionsCallback callback)
        {
            return var => callback(var)
            .Where(c => c.Value.StartsWith(var) && c.Value != var)
            .Select(c => new SuggestionModel(c.DisplayValue, c.Value.Remove(0, var.Length), SuggestionType.Autocomplete));
        }

        public static IEnumerable<SuggestionModel> EmptySuggestions(string var) => Enumerable.Empty<SuggestionModel>();


        public static SuggestionModelCallback With(this SuggestionModelCallback callback, SuggestionModelCallback callbackup2)
        {
            return (var) => callback(var).Concat(callbackup2(var));
        }

    }
}
