using System.Collections.Generic;

namespace Gint.SyntaxHighlighting
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

    internal abstract class ExpressionTraverser
    {
        public void Traverse(ExpressionSyntax rootNode)
        {
            EvaluateNode(rootNode);
        }

        private void EvaluateNode(ExpressionSyntax node)
        {
            if (node == null) return;
            switch (node.Kind)
            {
                case CommandTokenKind.CommandExpression:
                    EvaluateCommand((CommandExpressionSyntax)node);
                    break;
                case CommandTokenKind.CommandWithVariableExpression:
                    EvaluateCommandWithVariable((CommandWithVariableExpressionSyntax)node);
                    break;
                case CommandTokenKind.OptionExpression:
                    EvaluateOption((OptionExpressionSyntax)node);
                    break;
                case CommandTokenKind.VariableOptionExpression:
                    EvaluateVariableOption((VariableOptionExpressionSyntax)node);
                    break;
                case CommandTokenKind.PipeExpression:
                    EvaluatePipe((PipeExpressionSyntax)node);
                    break;
                case CommandTokenKind.PipedCommandExpression:
                    EvaluatePipeline((PipedCommandExpressionSyntax)node);
                    break;
            }
        }

        protected abstract void Pipe(PipeExpressionSyntax node);
        private void EvaluatePipe(PipeExpressionSyntax node)
        {
            Pipe(node);
        }

        protected abstract void VariableOption(VariableOptionExpressionSyntax node);
        private void EvaluateVariableOption(VariableOptionExpressionSyntax node)
        {
            VariableOption(node);
        }

        protected abstract void Option(OptionExpressionSyntax node);
        private void EvaluateOption(OptionExpressionSyntax node)
        {
            Option(node);
        }

        protected abstract void CommandWithVariable(CommandWithVariableExpressionSyntax node);
        private void EvaluateCommandWithVariable(CommandWithVariableExpressionSyntax node)
        {
            CommandWithVariable(node);
            foreach (var opt in node.Options)
            {
                EvaluateNode(opt);
            }
        }

        protected abstract void Command(CommandExpressionSyntax node);
        private void EvaluateCommand(CommandExpressionSyntax node)
        {
            Command(node);
            foreach (var opt in node.Options)
            {
                EvaluateNode(opt);
            }
        }

        private void EvaluatePipeline(PipedCommandExpressionSyntax node)
        {
            EvaluateNode(node.FirstCommand);
            EvaluatePipe(node.PipeToken);
            EvaluateNode(node.SecondCommand);
        }
    }


}
