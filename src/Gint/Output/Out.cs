using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using Gint.Markup;

namespace Gint
{
    internal class RemoveFromListDisposable<TEntry> : IDisposable
    {
        private List<TEntry> registered;
        private TEntry current;

        internal RemoveFromListDisposable(List<TEntry> registered, TEntry current)
        {
            this.registered = registered;
            this.current = current;
        }

        public void Dispose()
        {
            if (registered.Contains(current))
                registered.Remove(current);
        }
    }

    public class Out : MarkupDocument, IDisposable
    {
        public List<MarkupWriter> OutputWriters { get; }

        public Out()
        {
            OutputWriters = new List<MarkupWriter>();
        }

        public Out(IEnumerable<MarkupWriter> writters)
        {
            OutputWriters = writters.ToList();
        }

        public IDisposable AddWriter(MarkupWriter writer)
        {
            OutputWriters.Add(writer);
            return new RemoveFromListDisposable<MarkupWriter>(OutputWriters, writer);
        }

        public void RemoveWriter(MarkupWriter writer)
        {
            OutputWriters.Remove(writer);
        }
    
        public void Print()
        {
            var str = Buffer;
            foreach (var writter in OutputWriters)
            {
                writter.Print(str);
            }
        }

        public void Flush()
        {
            if (Buffer == string.Empty) return;

            var str = Buffer;
            Clear();

            foreach (var writter in OutputWriters)
            {
                writter.Print(str);
                writter.Flush();
            }         
        }

        public void WriteContentsTo(MarkupWriter writer)
        {
            var str = Buffer;
            writer.Print(str);
        }

        public void Dispose()
        {
            Flush();
        }
    }

}
