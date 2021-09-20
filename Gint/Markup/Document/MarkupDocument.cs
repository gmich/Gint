﻿using System;
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

        private string EscapeVariable(string variable)
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

        public CloseFormat StartFormatWithVariable(string token, string variable)
        {
            var newtoken = $"{token}{MarkupFormatConsts.FormatVariableStart}{EscapeVariable(variable)}";
            return StartFormat(newtoken);
        }


        public CloseFormat StartFormat(string token)
        {
            var formatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{token}{MarkupFormatConsts.FormatTagClose}";
            buffer.Append(formatWithTag);
            return new CloseFormat(() =>
            {
                var closeFormatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{MarkupFormatConsts.FormatEnd}{token}{MarkupFormatConsts.FormatTagClose}";
                buffer.Append(closeFormatWithTag);
            });
        }

        public CloseFormat StartFormat(string[] tokens)
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

        public void FormatToken(string token)
        {
            var formatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{MarkupFormatConsts.FormatToken}{token}{MarkupFormatConsts.FormatTagClose}";
            buffer.Append(formatWithTag);
        }

        public void FormatTokenWithVariable(string token, string variable)
        {
            var newtoken = $"{token}{MarkupFormatConsts.FormatVariableStart}{EscapeVariable(variable)}";
            FormatToken(newtoken);
        }

        public void CloseFormat(string token)
        {
            var closeFormatWithTag = $"{MarkupFormatConsts.FormatTagOpen}{MarkupFormatConsts.FormatEnd}{token}{MarkupFormatConsts.FormatTagClose}";
            buffer.Append(closeFormatWithTag);
        }


        public void Write(string text)
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
        }

        public string Document => buffer.ToString();

        public override string ToString() => Document;

        public void Clear() => buffer.Clear();

    }
}