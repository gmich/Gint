namespace Gint.Commands.Streaming
{
    public class UTF8StreamAdapter : IStreamAdapter<IUTF8Upstream, IUTF8Downstream>
    {
        public IUTF8Downstream Adapt(IUTF8Upstream upstream)
        {
            if (upstream is UTF8Upstream s)
            {
                var downstream = new UTF8Downstream(s.ToString());
                s.Builder.Clear();
                return downstream;
            }
            throw new StreamAdapterException("Unsupported upstream protocol.");
        }
  
    }
}
