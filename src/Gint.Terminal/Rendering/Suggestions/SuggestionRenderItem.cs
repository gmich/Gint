using System;

namespace Gint.Terminal
{
    internal class SuggestionRenderItem : Renderable
    {
        public SuggestionRenderItem(SuggestionModel suggestion)
        {
            RenderValue = suggestion.RenderValue;
            Suggestion = suggestion;
        }

        public bool HasFocus { get; set; }
        public SuggestionModel Suggestion { get; }

        public override void Render()
        {
            if (HasFocus)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
            }
            Console.Write(RenderValue);
            Console.ResetColor();
            HasFocus = false;
        }
    }
}
