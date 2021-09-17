using System;
using System.Collections.Generic;

namespace Gint.Markup
{

    public class ConsoleMarkupWriter : MarkupWriter
    {

        private readonly static Format.ConsoleMarkupWriterFormatFactory formatFactory = new();
        private readonly List<Format.IMarkupFormat> appliedFormats = new();

        public override bool OnLintingError(DiagnosticCollection diagnostics, string text)
        {
            foreach (var diagnostic in diagnostics)
            {
                var error = text.Substring(diagnostic.Location.Start, diagnostic.Location.Length);
                var prefix = text.Substring(0, diagnostic.Location.Start);
                var suffix = text[diagnostic.Location.End..];
                Console.Write(prefix);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(error);
                Console.ResetColor();
                Console.WriteLine(suffix);
                Console.WriteLine(diagnostic.Message);
                Console.WriteLine();
            }
            return false;
        }

        protected override void EndOfStream()
        {
            appliedFormats.Clear();
        }

        protected override void FormatEnd(string tag)
        {
            formatFactory.GetFormat(tag).Remove();
            for (int i = appliedFormats.Count - 1; 0 <= i; i--)
            {
                var format = appliedFormats[i];
                if(format.Tag == tag)
                {
                    format.Remove();
                    appliedFormats.Remove(format);
                }
            }
        }

        protected override void FormatStart(string tag)
        {
            var format = formatFactory.GetFormat(tag);
            format.Apply();
            appliedFormats.Add(format);
        }

        protected override void NewLine()
        {
            //Console.WriteLine();
        }

        protected override void PrintText(string text)
        {
            Console.Write(text);
        }

        protected override void StartOfStream()
        {
            appliedFormats.Clear();
        }
    }

}
