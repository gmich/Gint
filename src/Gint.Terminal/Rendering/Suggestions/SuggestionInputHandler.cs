using System;

namespace Gint.Terminal
{
    internal class SuggestionInputHandler
    {
        private SuggestionSelector selector;
        private SuggestionRenderer renderer;
        public event EventHandler OnChange;
        public event EventHandler<LostFocusEventArgs> OnLostFocus;
        public bool HasFocus { get; private set; }

        public void Handle(SuggestionRenderer renderer)
        {
            HasFocus = true;
            this.renderer = renderer;
            selector = renderer.Cursor;
                
            selector.OnCursorExit += (sender, args) =>
            {
                HasFocus = false;
                OnLostFocus?.Invoke(this, LostFocusEventArgs.Denied);
            };
            OnChange?.Invoke(this, EventArgs.Empty);
            HandleInput();
        }

     
        private void HandleInput()
        {
            Console.CursorVisible = false;
            while (HasFocus)
            {
                var beforeReadKeyBufferWidth = Console.BufferWidth;
                var key = Console.ReadKey(intercept: true);
                if (beforeReadKeyBufferWidth != Console.BufferWidth)
                {
                    LostFocus();
                    return;
                }

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
                        LostFocus();
                        return;
                }
                if (HasFocus)
                    OnChange?.Invoke(this, EventArgs.Empty);
            }
        }

        private void LostFocus()
        {
            HasFocus = false;
            OnLostFocus?.Invoke(this, LostFocusEventArgs.Denied);
        }

        private void SuggestionAccepted()
        {
            HasFocus = false;
            var suggestion = renderer.CurrentSuggestion;
            OnLostFocus?.Invoke(this, LostFocusEventArgs.Accepted(suggestion.Type, suggestion.Value));
        }

        public Action GenerateRenderCallback()
        {
            return renderer.GenerateRenderCallback();
        }

        private void DownArrowKeyPressed()
        {
            selector.MoveDown();
        }

        private void LeftArrowPressed()
        {
            selector.MoveLeft();
        }

        private void RightArrowPressed()
        {
            selector.MoveRight();
        }

        private void UpArrowKeyPressed()
        {
            selector.MoveUp();
        }
    }
}
