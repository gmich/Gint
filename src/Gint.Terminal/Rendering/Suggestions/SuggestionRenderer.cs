using System;
using System.Collections.Generic;
using System.Linq;

namespace Gint.Terminal
{
    internal class SuggestionRenderer
    {
        private readonly SuggestionModel[] suggestions;
        private readonly int maxSuggestionsPerRow;
        private readonly TerminalTheme theme;

        public List<SuggestionCollection> SuggestionList { get; } = new List<SuggestionCollection>();

        public SuggestionSelector Cursor { get; private set; }

        public SuggestionRenderer(SuggestionModel[] suggestions, int maxSuggestionsPerRow, TerminalTheme theme)
        {
            this.suggestions = suggestions;
            this.maxSuggestionsPerRow = maxSuggestionsPerRow;
            this.theme = theme;
            Reset();
        }

        public void Reset()
        {
            int index = 0;
            SuggestionList.Add(new SuggestionCollection());
            void AddTab()
            {
                SuggestionList[index].Renderables.Add(new Renderable { RenderValue = "     " });
            }

            for (int i = 0; i < suggestions.Length; i++)
            {
                AddTab();
                var sug = suggestions[i];
                if (SuggestionList[index].TotalRenderSize + sug.Length > Console.BufferWidth || SuggestionList[index].Count >= maxSuggestionsPerRow)
                {
                    SuggestionList.Add(new SuggestionCollection());
                    index++;
                    AddTab();
                }
                var suggestionItem = new SuggestionRenderItem(sug, theme);
                SuggestionList[index].Add(suggestionItem);
                SuggestionList[index].Renderables.Add(suggestionItem);
            }

            Cursor = new SuggestionSelector(this.SuggestionList);
        }

        public Action GenerateRenderCallback()
        {
            return new Action(() =>
            {
                Console.WriteLine();
                SuggestionList[Cursor.Row][Cursor.Column].HasFocus = true;

                for (int i = 0; i < SuggestionList.Count(); i++)
                {
                    for (int j = 0; j < SuggestionList[i].Renderables.Count(); j++)
                    {
                        SuggestionList[i].Renderables[j].Render();
                    }
                    if (i + 1 != SuggestionList.Count())
                        Console.WriteLine();
                }
            });
        }

        public SuggestionModel CurrentSuggestion => SuggestionList[Cursor.Row][Cursor.Column].Suggestion;
    }
}
