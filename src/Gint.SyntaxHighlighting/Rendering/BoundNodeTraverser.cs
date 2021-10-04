using System.Collections.Generic;

namespace Gint.SyntaxHighlighting
{
    internal class BoundNodeRenderItemTraverser : BoundNodeTraverser
    {
        private BoundNodeRenderItemTraverser() { }

        private readonly List<RenderItem> renderItems = new List<RenderItem>();

        public static RenderItem[] GetRenderItems(BoundNode rootNode)
        {
            var traverser = new BoundNodeRenderItemTraverser();
            traverser.Traverse(rootNode);
            return traverser.renderItems.ToArray();
        }

        protected override void BoundCommand(BoundCommand node)
        {
            renderItems.Add(new BoundRenderItem(node.TextSpan, node.Kind, node));
        }

        protected override void BoundCommandWithVariable(BoundCommandWithVariable node)
        {
            renderItems.Add(new BoundRenderItem(node.TextSpanWithVariable, node.Kind, node));
        }

        protected override void Option(BoundOption node)
        {
            renderItems.Add(new BoundRenderItem(node.TextSpan, node.Kind, node));
        }

        protected override void Pipe(BoundPipe node)
        {
            renderItems.Add(new BoundRenderItem(node.TextSpan, node.Kind, node));
        }

        protected override void VariableOption(BoundVariableOption node)
        {
            renderItems.Add(new BoundRenderItem(node.TextSpanWithVariable, node.Kind, node));
        }
    }

    internal abstract class BoundNodeTraverser
    {
        public void Traverse(BoundNode rootNode)
        {
            EvaluateNode(rootNode);
        }

        private void EvaluateNode(BoundNode node)
        {
            switch (node.Kind)
            {
                case BoundNodeKind.Command:
                    EvaluateCommand((BoundCommand)node);
                    break;
                case BoundNodeKind.CommandWithVariable:
                    EvaluateCommandWithVariable((BoundCommandWithVariable)node);
                    break;
                case BoundNodeKind.Option:
                    EvaluateOption((BoundOption)node);
                    break;
                case BoundNodeKind.VariableOption:
                    EvaluateVariableOption((BoundVariableOption)node);
                    break;
                case BoundNodeKind.Pipe:
                    EvaluatePipe((BoundPipe)node);
                    break;
                case BoundNodeKind.Pipeline:
                    EvaluatePipeline((BoundPipeline)node);
                    break;
            }
        }

        protected abstract void Pipe(BoundPipe node);
        private void EvaluatePipe(BoundPipe node)
        {
            Pipe(node);
        }

        protected abstract void VariableOption(BoundVariableOption node);
        private void EvaluateVariableOption(BoundVariableOption node)
        {
            VariableOption(node);
        }

        protected abstract void Option(BoundOption node);
        private void EvaluateOption(BoundOption node)
        {
            Option(node);
        }

        protected abstract void BoundCommandWithVariable(BoundCommandWithVariable node);
        private void EvaluateCommandWithVariable(BoundCommandWithVariable node)
        {
            BoundCommandWithVariable(node);
            foreach (var opt in node.BoundOptions)
            {
                EvaluateNode(opt);
            }
        }

        protected abstract void BoundCommand(BoundCommand node);
        private void EvaluateCommand(BoundCommand node)
        {
            BoundCommand(node);
            foreach (var opt in node.BoundOptions)
            {
                EvaluateNode(opt);
            }
        }

        private void EvaluatePipeline(BoundPipeline boundPipeline)
        {
            EvaluateNode(boundPipeline.FirstNode);
            EvaluatePipe(boundPipeline.Pipe);
            EvaluateNode(boundPipeline.SecondNode);
        }
    }


}
