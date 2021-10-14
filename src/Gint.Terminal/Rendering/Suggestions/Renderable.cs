using System;

namespace Gint.Terminal
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
