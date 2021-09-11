using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;

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

    public class Out : IDisposable
    {
        private readonly StringBuilder buffer = new StringBuilder();
        private bool formatting = false;

        public string Buffer => buffer.ToString();
        public List<OutputWriter> OutputWriters { get; }

        public static Out WithConsoleOutput => new Out().AddWriter(new ConsoleOutputWriter()).Out;

        public Out()
        {
            OutputWriters = new List<OutputWriter>();
        }

        public Out(IEnumerable<OutputWriter> writters)
        {
            OutputWriters = writters.ToList();
        }

        public (Out Out, IDisposable RemoveFromList) AddWriter(OutputWriter writer)
        {
            OutputWriters.Add(writer);
            return (this, new RemoveFromListDisposable<OutputWriter>(OutputWriters, writer));
        }

        public Out RemoveWriter(OutputWriter writer)
        {
            OutputWriters.Remove(writer);
            return this;
        }

        public Out Format(params FormatType[] outputEncodingTokens)
        {
            var formats = outputEncodingTokens.Select(c => FormatRegistry.GetFormatMaskWithPrefix(c));
            var joinedFormat = string.Join(string.Empty, formats);
            buffer.Append(joinedFormat);
            formatting = !outputEncodingTokens.Contains(FormatType.ResetFormat);
            return this;
        }

        public Out Format(params string[] masks)
        {
            var parsedMasks = masks.Select(mask => $"{FormatFacts.MaskPrefix}{mask}");
            var joinedParsedMasks = string.Join(string.Empty, parsedMasks);
            buffer.Append(joinedParsedMasks);
            formatting = !joinedParsedMasks.Contains(FormatRegistry.GetFormatMaskWithPrefix(FormatType.ResetFormat));
            return this;
        }

        public Out ClearFormat()
        {
            Format(FormatType.ResetFormat);
            return this;
        }

        public Out Write(string text)
        {
            var tempBuffer = new StringBuilder();
            foreach (var c in text)
            {
                tempBuffer.Append(c);
                if (c == FormatFacts.MaskPrefix)
                {
                    //escape it by adding it twice
                    tempBuffer.Append(FormatFacts.MaskPrefix);
                }
            }

            buffer.Append(tempBuffer.ToString());
            return this;
        }

        public Out WriteRaw(string text)
        {
            buffer.Append(text);
            return this;
        }

        public Out WriteWhitespace()
        {
            Write(" ");
            return this;
        }

        public Out WriteFormatted(string text, params FormatType[] outputEncodingTokens)
        {
            Format(outputEncodingTokens);
            Write(text);
            ClearFormat();
            return this;
        }

        public Out WriteFormatted(string text, params string[] outputEncodingMasks)
        {
            Format(outputEncodingMasks);
            Write(text);
            ClearFormat();
            return this;
        }

        public Out WriteLine(string text)
        {
            Write(text);
            return WriteLine();
        }

        public Out WriteLine()
        {
            buffer.Append(Environment.NewLine);
            return this;
        }

        public Out Print()
        {
            var str = Buffer;
            foreach (var writter in OutputWriters)
            {
                writter.Print(str);
            }
            return this;
        }

        public Out Flush()
        {
            if (Buffer == string.Empty) return this;

            if (formatting)
            {
                ClearFormat();
            }
            var str = Buffer;
            buffer.Clear();

            foreach (var writter in OutputWriters)
            {
                writter.Print(str);
                writter.Flush();
            }
            return this;
        }

        public Out WriteContentsTo(OutputWriter outputWriter)
        {
            var str = Buffer;
            outputWriter.Print(str);
            return this;
        }

        public void Dispose()
        {
            //flush and dispose
            Flush();
        }
    }

}
