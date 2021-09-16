using System;
using System.Collections.Generic;
using System.Linq;

namespace Gint.Markup
{
    public class ConsoleMarkupWriter : MarkupWriter
    {
        public override void Flush()
        {
            throw new NotImplementedException();
        }

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
        }

        protected override void FormatEnd(MarkupSyntaxToken token)
        {
        }

        protected override void FormatStart(MarkupSyntaxToken token)
        {
        }

        protected override void NewLine(MarkupSyntaxToken token)
        {
            Console.WriteLine();
        }

        protected override void PrintText(MarkupSyntaxToken token)
        {
            Console.Write(token.Value);
        }

        protected override void StartOfStream()
        {
        }
    }

}
