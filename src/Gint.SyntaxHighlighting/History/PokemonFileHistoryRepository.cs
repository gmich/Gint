using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gint.SyntaxHighlighting
{
    public class PokemonFileHistoryRepository : ICommandHistoryRepository
    {
        private readonly string filepath;

        public PokemonFileHistoryRepository(string filePath = "gint-cmd-history.log")
        {
            this.filepath = filePath;
        }

        public void Add(string entry)
        {
            try
            {
                File.AppendAllText(filepath, entry + Environment.NewLine);
            }
            catch { }
        }

        public IEnumerable<string> Load()
        {
            try
            {
                if (File.Exists(filepath))
                {
                    return File.ReadAllLines(filepath);
                }
                return Enumerable.Empty<string>();
            }
            catch
            {
                return Enumerable.Empty<string>();

            }
        }
    }
}
