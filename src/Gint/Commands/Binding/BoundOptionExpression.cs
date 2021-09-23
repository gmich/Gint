namespace Gint
{
    internal abstract class BoundOptionExpression : BoundNode
    {
        protected BoundOptionExpression(string argument, bool allowMultiple, TextSpan span)
        {
            Argument = argument;
            AllowMultiple = allowMultiple;
            TextSpan = span;
        }

        public string Argument { get; }
        public TextSpan TextSpan { get; }
        public bool AllowMultiple { get; }
        public abstract int Priority { get; }
    }
}
