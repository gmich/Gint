using System;

namespace Gint.SyntaxHighlighting
{
    internal class Renderable
    {
        public string RenderValue { get; init; }
        public int Length => RenderValue.Length;

        public virtual void Render()
        {
            Console.Write(RenderValue);
        }
    }
}
