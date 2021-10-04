namespace Gint.SyntaxHighlighting
{
    internal sealed class ErrorRenderItem : RenderItem
    {
        public ErrorRenderItem(TextSpan location, Diagnostic diagnostic) : base(location)
        {
            Diagnostic = diagnostic;
        }

        public Diagnostic Diagnostic { get; }

        public override RenderItemType RenderItemType => RenderItemType.Error;
    }

}
