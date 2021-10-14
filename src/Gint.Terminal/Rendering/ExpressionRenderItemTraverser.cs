using System.Collections.Generic;

namespace Gint.Terminal
{
    internal class ExpressionRenderItemTraverser : ExpressionTraverser
    {
        private ExpressionRenderItemTraverser() { }

        private readonly List<RenderItem> renderItems = new List<RenderItem>();

        public static RenderItem[] GetRenderItems(ExpressionSyntax rootNode)
        {
            var traverser = new ExpressionRenderItemTraverser();
            traverser.Traverse(rootNode);
            return traverser.renderItems.ToArray();
        }

        protected override void Command(CommandExpressionSyntax node)
        {
            renderItems.Add(new ExpressionRenderItem(node.Span, node.Kind, node));
        }

        protected override void CommandWithVariable(CommandWithVariableExpressionSyntax node)
        {
            renderItems.Add(new ExpressionRenderItem(node.CommandToken.Span, node.Kind, node));
            renderItems.Add(new ExpressionRenderItem(node.VariableToken.Span, node.VariableToken.Kind, node));
        }

        protected override void Option(OptionExpressionSyntax node)
        {
            renderItems.Add(new ExpressionRenderItem(node.Span, node.Kind, node));
        }

        protected override void Pipe(PipeExpressionSyntax node)
        {
            renderItems.Add(new ExpressionRenderItem(node.Span, node.Kind, node));
        }

        protected override void VariableOption(VariableOptionExpressionSyntax node)
        {
            renderItems.Add(new ExpressionRenderItem(node.OptionToken.Span, node.Kind, node));
            renderItems.Add(new ExpressionRenderItem(node.VariableToken.Span, node.VariableToken.Kind, node));
        }
    }


}
