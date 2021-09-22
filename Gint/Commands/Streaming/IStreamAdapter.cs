namespace Gint.Commands.Streaming
{
    public interface IStreamAdapter<Upstream, Downstream>
        where Upstream : IUpstream
        where Downstream : IDownstream
    {
        Downstream Adapt(Upstream upstream);
    }
}
