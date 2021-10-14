namespace Gint
{
    public struct Suggestion
    {
        public Suggestion(string value, string displayValue)
        {
            Value = value;
            DisplayValue = displayValue;
        }

        public string Value { get; }
        public string DisplayValue { get; }
    }
}
