namespace Gint
{
    internal abstract class BoundOptionExpression : BoundNode
    {
        public abstract int Priority { get; }
    }
}
