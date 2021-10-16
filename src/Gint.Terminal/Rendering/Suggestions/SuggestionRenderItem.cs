using System;

namespace Gint.Terminal
{
    internal class SuggestionRenderItem : Renderable
    {
        private readonly TerminalTheme theme;
     
        public SuggestionRenderItem(SuggestionModel suggestion, TerminalTheme theme)
        {
            RenderValue = suggestion.RenderValue;
            Suggestion = suggestion;
            this.theme = theme;
        }

        public bool HasFocus { get; set; }
        public SuggestionModel Suggestion { get; }

        public override void Render()
        {
            if (HasFocus)
            {
                theme.SuggestionFocus.Apply();
            }
            else
            {
                theme.Suggestion.Apply();
            }
            Console.Write(RenderValue);
            Console.ResetColor();
            HasFocus = false;
        }
    }
}
