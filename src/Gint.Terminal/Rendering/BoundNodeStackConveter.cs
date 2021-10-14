using System.Collections.Generic;

namespace Gint.Terminal
{
    internal class BoundNodeStackConveter : BoundNodeTraverser
    {
        private Stack<BoundNode> Nodes { get; } = new Stack<BoundNode>();

        private BoundNodeStackConveter() { }

        public static Stack<BoundNode> GetBoundNodeStack(BoundNode node)
        {
            var converter = new BoundNodeStackConveter();
            converter.Traverse(node);
            return converter.Nodes;
        }

        protected override void Command(BoundCommand node)
        {
            Nodes.Push(node);
        }

        protected override void CommandWithVariable(BoundCommandWithVariable node)
        {
            Nodes.Push(node);
        }

        protected override void Option(BoundOption node)
        {
            Nodes.Push(node);
        }

        protected override void Pipe(BoundPipe node)
        {
            Nodes.Push(node);
        }

        protected override void VariableOption(BoundVariableOption node)
        {
            Nodes.Push(node);
        }
    }
}