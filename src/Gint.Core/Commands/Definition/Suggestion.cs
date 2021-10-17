namespace Gint
{
    public struct Suggestion
    {
        public Suggestion(string value) : this(value, value)
        {

        }

        public Suggestion(string value, string displayValue)
        {
            Value = value;
            DisplayValue = displayValue;
        }

        public string Value { get; init; }
        public string DisplayValue { get; init; }

        public static implicit operator Suggestion(string val)
        {
            return new Suggestion(val);
        }
    }
}
