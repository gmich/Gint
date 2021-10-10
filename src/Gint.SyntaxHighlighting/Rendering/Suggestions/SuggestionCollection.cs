using System.Collections.Generic;
using System.Linq;

namespace Gint.SyntaxHighlighting
{
    internal class SuggestionCollection : List<SuggestionRenderItem>
    {
        public List<Renderable> Renderables { get; } = new List<Renderable>();
        public int TotalRenderSize => Renderables.Sum(c => c.Length);
    }
}
