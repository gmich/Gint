using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gint.SyntaxHighlighting
{
    public class ConsoleInputManager
    {
        private readonly ConsoleInputInterceptor consoleInputInterceptor;
        private readonly Prompt prompt;
        private readonly CommandRenderer renderer;
        private readonly CommandHistory history;

        private VirtualCursor virtualCursor;
        private ConsoleWindowResizer consoleWindowResizer;
        private SystemConsoleAdapter systemConsoleAdapter;
        private CommandText commandText;

        public event EventHandler<string> OnCommandReady;

        private int GetTotalSize() => commandText.Value.Length;
        private bool requiresReset = true;

        public ConsoleInputManager()
        {
            prompt = new Prompt("cli>");
            consoleInputInterceptor = new ConsoleInputInterceptor();
            renderer = new CommandRenderer();
            history = new CommandHistory(limit: 20);
            Reset();
        }

        private void CreateActors()
        {
            virtualCursor = new VirtualCursor(GetTotalSize, prompt);
            consoleWindowResizer = new ConsoleWindowResizer(virtualCursor);
            systemConsoleAdapter = new SystemConsoleAdapter(prompt, consoleWindowResizer, virtualCursor);
            commandText = new CommandText(virtualCursor);
        }

        private void Reset()
        {
            CreateActors();

            commandText.OnChange += (sender, args) =>
            {
                var totalCharactersWritten = args.Previous.Length + prompt.Length + renderer.SuggestionLength;
                systemConsoleAdapter.ClearConsoleInput(totalCharactersWritten);
                prompt.Print();
                renderer.Render(commandText.Value);
                systemConsoleAdapter.AdjustToVirtualCursor();
            };

            virtualCursor.OnPositionChanged += (sender, args) =>
            {
                systemConsoleAdapter.AdjustToVirtualCursor();
            };

            OnCommandReady += (sender, args) =>
            {
                 history.Record(args);
            };

            commandText.Clear();

            requiresReset = false;
        }

        public void WaitNext()
        {
            if (requiresReset) Reset();

            consoleWindowResizer.RecordBufferState();
            var key = consoleInputInterceptor.GetNextKey();
            consoleWindowResizer.AdjustIfNeeded();

            HandleKey(key);
        }

        private void HandleKey(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    EnterKeyPressed();
                    break;
                case ConsoleKey.Tab:
                    TabKeyPressed();
                    break;
                case ConsoleKey.UpArrow:
                    UpArrowKeyPressed();
                    break;
                case ConsoleKey.DownArrow:
                    DownArrowKeyPressed();
                    break;
                case ConsoleKey.PageUp:
                    PageUpPressed();
                    break;
                case ConsoleKey.PageDown:
                    PageDownPressed();
                    break;
                case ConsoleKey.LeftArrow:
                    LeftArrowPressed();
                    break;
                case ConsoleKey.RightArrow:
                    RightArrowPressed();
                    break;
                case ConsoleKey.Delete:
                    DeleteKeyPressed();
                    break;
                case ConsoleKey.Backspace:
                    BackspaceKeyPressed();
                    break;
                case ConsoleKey.Escape:
                    EscapeArrowPressed();
                    break;
                default:
                    if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.V)
                    {
                        CopyCombinationPressed();
                    }
                    else
                    {
                        CharacterKeyPress(key);
                    }

                    break;
            }
        }

        private void EnterKeyPressed()
        {
            if (string.IsNullOrEmpty(commandText.Value)) return;

            OnCommandReady?.Invoke(this, commandText.Value);
            systemConsoleAdapter.NewLine();
            requiresReset = true;
        }

        private void TabKeyPressed()
        {
            throw new NotImplementedException();
        }

        private void UpArrowKeyPressed()
        {
            throw new NotImplementedException();
        }

        private void DownArrowKeyPressed()
        {
            throw new NotImplementedException();
        }

        private void PageUpPressed()
        {
            throw new NotImplementedException();
        }

        private void PageDownPressed()
        {
            throw new NotImplementedException();
        }

        private void LeftArrowPressed()
        {
            virtualCursor.Back();
        }

        private void RightArrowPressed()
        {
            virtualCursor.Forward();
        }

        private void DeleteKeyPressed()
        {
            commandText.RemoveNextCharacter();
        }

        private void BackspaceKeyPressed()
        {
            commandText.RemoveCurrentCharacter();
            virtualCursor.Back();
        }

        private void EscapeArrowPressed()
        {
            commandText.Clear();
            virtualCursor.Reset();
        }

        private void CopyCombinationPressed()
        {
            throw new NotImplementedException();
        }

        private void CharacterKeyPress(ConsoleKeyInfo key)
        {
            commandText.InsertCharacter(key.KeyChar);
            virtualCursor.Forward();
        }
    }
}
