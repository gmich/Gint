using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gint
{
    public class OutTextWriterAdapter : TextWriter
    {
        public OutTextWriterAdapter(Out @out)
        {
            Out = @out;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public Out Out { get; }

        public override void Write(string value)
        {
            Out.Write(value);
        }

        public override void WriteLine()
        {
            Out.WriteLine();
        }

        public override void Flush()
        {
            Out.Flush();
        }
    }
}
