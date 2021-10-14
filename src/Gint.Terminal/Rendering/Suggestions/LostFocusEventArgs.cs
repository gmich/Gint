namespace Gint.Terminal
{
    public class LostFocusEventArgs
    {
        public LostFocusEventArgs(bool suggestionAccepted, string value, SuggestionType suggestionType)
        {
            SuggestionAccepted = suggestionAccepted;
            Value = value;
            SuggestionType = suggestionType;
        }

        public bool SuggestionAccepted { get; }
        public string Value { get; }

        public SuggestionType SuggestionType { get; }
        public static LostFocusEventArgs Denied => new LostFocusEventArgs(false, null, SuggestionType.Autocomplete);
        public static LostFocusEventArgs Accepted(SuggestionType suggestionType, string value) => new LostFocusEventArgs(true, value, suggestionType);
    }
}
