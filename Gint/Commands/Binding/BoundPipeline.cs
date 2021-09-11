namespace Gint
{
    internal sealed class BoundPipeline : BoundNode
    {
        public BoundPipeline(BoundNode firstNode, BoundPipe pipe, BoundNode secondNode)
        {
            FirstNode = firstNode;
            Pipe = pipe;
            SecondNode = secondNode;
        }

        public override BoundNodeKind Kind { get; } = BoundNodeKind.Pipeline;
        public BoundNode FirstNode { get; }
        public BoundPipe Pipe { get; }
        public BoundNode SecondNode { get; }
    }

}
