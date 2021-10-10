namespace Gint.SyntaxHighlighting
{
    internal struct SuggestionModel
    {
        public SuggestionModel(string renderValue, string value, SuggestionType type)
        {
            RenderValue = renderValue;
            Value = value;
            Type = type;
        }

        public string RenderValue { get; init; }
        public string Value { get; init; }
        public SuggestionType Type { get; init; }
        public int Length => RenderValue.Length;
    }
}
