namespace Gint.SyntaxHighlighting
{

    internal abstract class BoundNodeTraverser
    {
        public void Traverse(BoundNode rootNode)
        {
            EvaluateNode(rootNode);
        }

        private void EvaluateNode(BoundNode node)
        {
            if (node == null) return;
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

        protected abstract void CommandWithVariable(BoundCommandWithVariable node);
        private void EvaluateCommandWithVariable(BoundCommandWithVariable node)
        {
            CommandWithVariable(node);
            foreach (var opt in node.BoundOptions)
            {
                EvaluateNode(opt);
            }
        }

        protected abstract void Command(BoundCommand node);
        private void EvaluateCommand(BoundCommand node)
        {
            Command(node);
            foreach (var opt in node.BoundOptions)
            {
                EvaluateNode(opt);
            }
        }

        private void EvaluatePipeline(BoundPipeline node)
        {
            EvaluateNode(node.FirstNode);
            EvaluatePipe(node.Pipe);
            EvaluateNode(node.SecondNode);
        }


    }
}