using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gint
{
    using Markup;

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
        public event EventHandler OnChange;

        public static MarkupDocument New => new MarkupDocument();

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
                if (c == MarkupFormatConsts.FormatSeparator)
                {
                    //escape it by adding it twice
                    tempBuffer.Append(MarkupFormatConsts.FormatSeparator);
                }
            }
            return tempBuffer.ToString();
        }

        private void Add(string str)
        {
            buffer.Append(str);
            OnChange?.Invoke(this,EventArgs.Empty);
        }

        private void Add(StringBuilder builder)
        {
            buffer.Append(builder);
            OnChange?.Invoke(this, EventArgs.Empty);
        }

        public CloseFormat AddFormatWithVariable(string token, string variable)
        {
            var newtoken = $"{token}{MarkupFormatConsts.FormatVariableStart}{EscapeVariable(variable)}";
            return AddFormatInternal(newtoken,token);
        }

        public CloseFormat AddFormat(string token)
        {
            return AddFormatInternal(token, null);
        }

        private CloseFormat AddFormatInternal(string token, string tokenWithoutVariable)
        {
            var formatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{token}{MarkupFormatConsts.FormatTagClose}";
            Add(formatWithTag);
            return new CloseFormat(() =>
            {
                var closeFormatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{MarkupFormatConsts.FormatEnd}{tokenWithoutVariable ?? token}{MarkupFormatConsts.FormatTagClose}";
                Add(closeFormatWithTag);
            });
        }

        public CloseFormat AddFormat(params string[] tokens)
        {
            var formatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{tokens.Aggregate((fst, snd) => $"{fst}{MarkupFormatConsts.FormatSeparator}{snd}")}{MarkupFormatConsts.FormatTagClose}";
            Add(formatWithTag);
            return new CloseFormat(() =>
            {
                var closeFormat = tokens
                .Select(c => $"{MarkupFormatConsts.FormatEnd}{c}")
                .Aggregate((fst, snd) => $"{fst}{MarkupFormatConsts.FormatSeparator}{snd}");

                var closeFormatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{closeFormat}{MarkupFormatConsts.FormatTagClose}";
                Add(closeFormatWithTag);
            });
        }

        public CloseFormat AddFormatWithVariable(params (string Token, string Variable)[] tokens)
        {
            var format = tokens
                .Select(c => $"{c.Token}{MarkupFormatConsts.FormatVariableStart}{EscapeVariable(c.Variable)}")
                .Aggregate((fst, snd) => $"{fst}{MarkupFormatConsts.FormatSeparator}{snd}");

            var formatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{format}{MarkupFormatConsts.FormatTagClose}";
            Add(formatWithTag);
            return new CloseFormat(() =>
            {
                var closeFormat = tokens
                .Select(c => $"{MarkupFormatConsts.FormatEnd}{c.Token}")
                .Aggregate((fst, snd) => $"{fst}{MarkupFormatConsts.FormatSeparator}{snd}");

                var closeFormatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{closeFormat}{MarkupFormatConsts.FormatTagClose}";
                Add(closeFormatWithTag);
            });
        }

        public void AddFormatToken(string token)
        {
            var formatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{MarkupFormatConsts.FormatToken}{token}{MarkupFormatConsts.FormatTagClose}";
            Add(formatWithTag);
        }

        public void AddFormatTokenWithVariable(string token, string variable)
        {
            var newtoken = $"{token}{MarkupFormatConsts.FormatVariableStart}{EscapeVariable(variable)}";
            AddFormatToken(newtoken);
        }

        public void CloseFormat(string token)
        {
            var closeFormatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{MarkupFormatConsts.FormatEnd}{token}{MarkupFormatConsts.FormatTagClose}";
            Add(closeFormatWithTag);
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
            Add(tempBuffer);
            return this;
        }

        public MarkupDocument WriteRaw(string text)
        {
            Add(text);
            return this;
        }

        public MarkupDocument Flush()
        {
            if (this is Out o)
                o.Flush();
            return this;
        }

        internal bool GetAutoflushFlag()
        {
            if (this is Out o)
                return o.AutoFlush;
            return false;
        }

        internal void SetAutoflush(bool b)
        {
            if (this is Out o)
                o.AutoFlush = b;
        }

        public string Buffer => buffer.ToString();

        public override string ToString() => Buffer;

        public void Clear() => buffer.Clear();

    }
}
