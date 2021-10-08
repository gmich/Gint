using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gint.SyntaxHighlighting
{
    internal class SuggestionItem
    {
        public SuggestionItem(string renderValue, string value)
        {
            RenderValue = renderValue;
            Value = value;
        }

        public string RenderValue { get; }
        public string Value { get; }

        public int Width => RenderValue.Length;

        public bool HasFocus { get; set; }
        public void Render()
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

    internal class SuggestionRenderer
    {
        public SuggestionItem[,] suggestionItems;
        private SuggestionCursor cursor;

        public void Init()
        {
            HasFocus = true;
            cursor = new SuggestionCursor(1, 3);
            suggestionItems = new SuggestionItem[2, 4]
            {
                { GetSuggestion, GetSuggestion, GetSuggestion2, GetSuggestion },
                { GetSuggestion, GetSuggestion2, GetSuggestion, GetSuggestion }
            };
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
            var suggestionValue = suggestionItems[cursor.Row, cursor.Column].Value;
            OnLostFocus?.Invoke(this, LostFocusEventArgs.Accepted(suggestionValue));
        }

        public void Render()
        {
            Console.WriteLine();
            var tab = "     ";
            suggestionItems[cursor.Row, cursor.Column].HasFocus = true;

            for (int i = 0; i < suggestionItems.GetLength(0); i++)
            {
                Console.Write(tab);

                for (int j = 0; j < suggestionItems.GetLength(1); j++)
                {
                    suggestionItems[i, j].Render();
                    Console.Write(tab);
                }
                Console.WriteLine();
            }
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

        private SuggestionItem GetSuggestion2 => new SuggestionItem("test2", "test2");
        private SuggestionItem GetSuggestion => new SuggestionItem("test", "test");
    }

    internal class SuggestionCursor
    {
        public SuggestionCursor(int height, int width)
        {
            MaxRow = height;
            MaxColumn = width;
        }

        public int MaxColumn { get; }
        public int MaxRow { get; }

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

        public void MoveDown()
        {
            Row = Math.Min(Row + 1, MaxRow);
        }

        public void MoveLeft()
        {
            Column = Math.Max(Column - 1, 0);
        }

        public void MoveRight()
        {
            Column = Math.Min(Column + 1, MaxColumn);
        }

    }
}
