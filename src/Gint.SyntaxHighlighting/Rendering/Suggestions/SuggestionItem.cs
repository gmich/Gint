using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gint.SyntaxHighlighting
{
    internal class SuggestionItem : Renderable
    {
        public SuggestionItem(string value)
        {
            Value = value;
        }

        public bool HasFocus { get; set; }
        public override void Render()
        {
            if (HasFocus)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
            }
            Console.Write(Value);
            Console.ResetColor();
            HasFocus = false;
        }
    }

    internal class Renderable
    {
        public string Value { get; init; }
        public int Length => Value.Length;

        public virtual void Render()
        {
            Console.Write(Value);
        }
    }


    public class LostFocusEventArgs
    {
        public LostFocusEventArgs(bool suggestionAccepted, string value)
        {
            SuggestionAccepted = suggestionAccepted;
            Value = value;
        }

        public bool SuggestionAccepted { get; }
        public string Value { get; }

        public static LostFocusEventArgs Denied => new LostFocusEventArgs(false, null);
        public static LostFocusEventArgs Accepted(string value) => new LostFocusEventArgs(true, value);
    }

    internal record SuggestionRecord
    {
        public string Value { get; init; }
        public string ExtraInfo { get; init; }
        public int Length => Value.Length + (ExtraInfo?.Length ?? 0);
    }

    internal class SuggestionList : List<SuggestionItem>
    {
        public List<Renderable> Renderables = new List<Renderable>();

        public int TotalRenderSize => Renderables.Sum(c => c.Length);
    }


    internal class SuggestionContainer
    {
        public List<SuggestionList> SuggestionList { get; } = new List<SuggestionList>();

        public SuggestionCursor Cursor { get; }

        public int MaxRenderablesPerRow = 5;
        public SuggestionContainer(SuggestionRecord[] suggestions)
        {
            int index = 0;
            SuggestionList.Add(new SuggestionList());
            void AddTab()
            {
                SuggestionList[index].Renderables.Add(new Renderable { Value = "     " });
            }

            for (int i = 0; i < suggestions.Length; i++)
            {
                AddTab();
                var sug = suggestions[i];
                if (SuggestionList[index].TotalRenderSize + sug.Length > Console.BufferWidth || SuggestionList[index].Count >= MaxRenderablesPerRow)
                {
                    SuggestionList.Add(new SuggestionList());
                    index++;
                    AddTab();
                }
                var suggestionItem = new SuggestionItem(sug.Value);
                SuggestionList[index].Add(suggestionItem);
                SuggestionList[index].Renderables.Add(suggestionItem);
            }

            Cursor = new SuggestionCursor(this.SuggestionList);
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

        public string Value => SuggestionList[Cursor.Row][Cursor.Column].Value;
    }


    internal class SuggestionRenderer
    {
        private SuggestionCursor cursor;
        private SuggestionContainer suggestionContainer;

        public void Init(SuggestionRecord[] suggestions)
        {
            HasFocus = true;
            suggestionContainer = new SuggestionContainer(suggestions);
            cursor = suggestionContainer.Cursor;

            cursor.OnCursorExit += (sender, args) =>
            {
                HasFocus = false;
                OnLostFocus?.Invoke(this, LostFocusEventArgs.Denied);
            };
            OnChange?.Invoke(this, EventArgs.Empty);
            HandleInput();
        }

        public event EventHandler OnChange;
        public event EventHandler<LostFocusEventArgs> OnLostFocus;
        public bool HasFocus { get; private set; }

        public void HandleInput()
        {
            Console.CursorVisible = false;
            while (HasFocus)
            {
                var key = Console.ReadKey(intercept: true);

                switch (key.Key)
                {
                    case ConsoleKey.Tab:
                    case ConsoleKey.Enter:
                        SuggestionAccepted();
                        return;
                    case ConsoleKey.UpArrow:
                        UpArrowKeyPressed();
                        break;
                    case ConsoleKey.DownArrow:
                        DownArrowKeyPressed();
                        break;
                    case ConsoleKey.LeftArrow:
                        LeftArrowPressed();
                        break;
                    case ConsoleKey.RightArrow:
                        RightArrowPressed();
                        break;
                    default:
                        HasFocus = false;
                        OnLostFocus?.Invoke(this, LostFocusEventArgs.Denied);
                        return;
                }
                if (HasFocus)
                    OnChange?.Invoke(this, EventArgs.Empty);
            }
        }

        private void SuggestionAccepted()
        {
            HasFocus = false;
            var suggestionValue = suggestionContainer.Value;
            OnLostFocus?.Invoke(this, LostFocusEventArgs.Accepted(suggestionValue));
        }

        public Action GenerateRenderCallback()
        {
            return suggestionContainer.GenerateRenderCallback();
        }

        private void DownArrowKeyPressed()
        {
            cursor.MoveDown();
        }

        private void LeftArrowPressed()
        {
            cursor.MoveLeft();
        }

        private void RightArrowPressed()
        {
            cursor.MoveRight();
        }

        private void UpArrowKeyPressed()
        {
            cursor.MoveUp();
        }
    }

    internal class SuggestionCursor
    {
        private readonly List<SuggestionList> suggestions;

        public SuggestionCursor(List<SuggestionList> suggestions)
        {
            this.suggestions = suggestions;
        }

        public int Column { get; private set; }
        public int Row { get; private set; }

        public event EventHandler OnCursorExit;

        public void MoveUp()
        {
            if (Row == 0)
            {
                OnCursorExit?.Invoke(this, EventArgs.Empty);
                return;
            }
            Row--;
        }

        private int ColumnMax => suggestions[Row].Count - 1;
        private int RowMax => suggestions.Count - 1;

        public void MoveDown()
        {
            Row = Math.Min(Row + 1, RowMax);
            Column = Math.Min(Column, ColumnMax);
        }

        public void MoveLeft()
        {
            Column = Math.Max(Column - 1, 0);
        }

        public void MoveRight()
        {
            Column = Math.Min(Column + 1, ColumnMax);
        }



    }
}
