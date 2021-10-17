using Gint.Pipes;
using System.Collections.Generic;

namespace Gint
{
    public class CommandScope
    {
        internal CommandScope(IPipeWriter pipeWriter, IPipeReader pipeReader)
        {
            PipeWriter = pipeWriter;
            PipeReader = pipeReader;
        }

        public IPipeWriter PipeWriter { get; }
        public IPipeReader PipeReader { get; }
        public Dictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

        public string ReadInputAsString(bool advanceCursor = true)
        {
            var res = PipeReader.Read(advanceCursor);
            return res.Buffer.ToUTF8String();
        }

        public void WriteString(string str)
        {
            PipeWriter.Write(str.ToUTF8EncodedByteArray());
        }

        public void WriteString(MarkupDocument document)
        {
            PipeWriter.Write(document.Buffer.ToUTF8EncodedByteArray());
            document.Clear();
        }

        public void Add(string key, object value)
        {
            Metadata.Add(key, value);
        }

        public T Get<T>(string key)
        {
            return (T)Metadata[key];
        }

        public T GetOrDefault<T>(string key, T def)
        {
            if (Metadata.ContainsKey(key))
            {
                return Get<T>(key);
            }
            return def;
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (Metadata.ContainsKey(key))
            {
                value = (T)Metadata[key];
                return true;
            }
            value = default;
            return false;

        }
    }

}
