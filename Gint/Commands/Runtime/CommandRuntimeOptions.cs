using Gint;
using System;
using System.IO;

namespace Gint
{
    public class CommandRuntimeOptions
    {
        public bool LogParseTree { get; set; }
        public bool LogBindTree { get; set; }

        public Out Out { get; set; } = new Out();
    }
}
