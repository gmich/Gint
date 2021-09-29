using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gint.Markup
{
    public class CloseFormat
    {
        private readonly Action close;

        internal CloseFormat(Action close)
        {
            this.close = close;
        }
        public void Close() => close();
    }

    public class MarkupDocument
    {
        private readonly StringBuilder buffer = new();

        private static string EscapeVariable(string variable)
        {
            var tempBuffer = new StringBuilder();
            foreach (var c in variable)
            {
                tempBuffer.Append(c);
                if (c == MarkupFormatConsts.FormatTagClose)
                {
                    //escape it by adding it twice
                    tempBuffer.Append(MarkupFormatConsts.FormatTagClose);
                }
            }
            return tempBuffer.ToString();
        }

        public CloseFormat AddFormatWithVariable(string token, string variable)
        {
            var newtoken = $"{token}{MarkupFormatConsts.FormatVariableStart}{EscapeVariable(variable)}";
            return AddFormat(newtoken);
        }


        public CloseFormat AddFormat(string token)
        {
            var formatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{token}{MarkupFormatConsts.FormatTagClose}";
            buffer.Append(formatWithTag);
            return new CloseFormat(() =>
            {
                var closeFormatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{MarkupFormatConsts.FormatEnd}{token}{MarkupFormatConsts.FormatTagClose}";
                buffer.Append(closeFormatWithTag);
            });
        }

        public CloseFormat AddFormat(string[] tokens)
        {
            var formatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{tokens.Aggregate((fst, snd) => $"{fst}{MarkupFormatConsts.FormatSeparator}{snd}")}{MarkupFormatConsts.FormatTagClose}";
            buffer.Append(formatWithTag);
            return new CloseFormat(() =>
            {
                var closeFormat = tokens
                .Select(c => $"{MarkupFormatConsts.FormatEnd}{c}")
                .Aggregate((fst, snd) => $"{fst}{MarkupFormatConsts.FormatSeparator}{snd}");

                var closeFormatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{closeFormat}{MarkupFormatConsts.FormatTagClose}";
                buffer.Append(closeFormatWithTag);
            });
        }

        public void AddFormatToken(string token)
        {
            var formatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{MarkupFormatConsts.FormatToken}{token}{MarkupFormatConsts.FormatTagClose}";
            buffer.Append(formatWithTag);
        }

        public void AddFormatTokenWithVariable(string token, string variable)
        {
            var newtoken = $"{token}{MarkupFormatConsts.FormatVariableStart}{EscapeVariable(variable)}";
            AddFormatToken(newtoken);
        }

        public void CloseFormat(string token)
        {
            var closeFormatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{MarkupFormatConsts.FormatEnd}{token}{MarkupFormatConsts.FormatTagClose}";
            buffer.Append(closeFormatWithTag);
        }

        public void CloseFormat()
        {
            CloseFormat(string.Empty);
        }

        public MarkupDocument Write(string text)
        {
            var tempBuffer = new StringBuilder();
            foreach (var c in text)
            {
                tempBuffer.Append(c);
                if (c == MarkupFormatConsts.FormatTagOpen)
                {
                    //escape it by adding it twice
                    tempBuffer.Append(MarkupFormatConsts.FormatTagOpen);
                }
            }
            buffer.Append(tempBuffer);
            return this;
        }

        public MarkupDocument WriteRaw(string text)
        {
            buffer.Append(text);
            return this;
        }

        public string Buffer => buffer.ToString();

        public override string ToString() => Buffer;

        public void Clear() => buffer.Clear();

    }
}
