namespace Gint.Terminal
{
    internal abstract class RenderItem
    {
        public RenderItem(TextSpan location)
        {
            Location = location;
        }

        public TextSpan Location { get; }
        public abstract RenderItemType RenderItemType { get; }
    }


}
