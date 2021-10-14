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
            OnChange += (sender, args) =>
            {
                if (AutoFlush)
                {
                    if (Buffer.Length >= FlushLimit)
                        Flush();
                }
            };
        }

        public event EventHandler<string> OnFlush;
        public bool AutoFlush { get; set; } = false;

        /// <summary>
        /// Flush when buffer is equal or greater than character count
        /// </summary>
        public int FlushLimit { get; set; } = 0;

        public static Out WithConsoleWriter
        {
            get
            {
                var writer = new ConsoleMarkupWriter();
                var o = new Out();
                o.AddWriter(writer);
                return o;
            }
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
            OnFlush?.Invoke(this, str);
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
