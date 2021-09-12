namespace Gint
{
    internal abstract class BoundOptionExpression : BoundNode
    {
        protected BoundOptionExpression(string argument)
        {
            Argument = argument;
        }

        public string Argument { get; }
        public abstract int Priority { get; }
    }
}
