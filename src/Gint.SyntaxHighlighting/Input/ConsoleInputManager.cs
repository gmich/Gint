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
            prompt = new Prompt("cli> ");
            consoleInputInterceptor = new ConsoleInputInterceptor();
            renderer = new CommandRenderer();
            history = new CommandHistory(limit: 20);
            Reset();

            OnCommandReady += (sender, args) =>
            {
                history.Record(args);
            };
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
                var renderCallback = renderer.GenerateRenderCallback(commandText.Value);
                systemConsoleAdapter.ClearConsoleInput(totalCharactersWritten);
                prompt.Print();
                renderCallback();
                systemConsoleAdapter.AdjustToVirtualCursor();
            };

            virtualCursor.OnPositionChanged += (sender, args) =>
            {
                systemConsoleAdapter.AdjustToVirtualCursor();
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
                    //No need, .net console handles paste is as a stream of characters so it works
                    //if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.V)
                    //    CopyCombinationPressed();
                    //else
                        CharacterKeyPress(key);

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
        }

        private void UpArrowKeyPressed()
        {
            if (history.GetPrevious(out var command))
            {
                commandText.Replace(command);
                virtualCursor.Forward(command.Length);
            }
        }

        private void DownArrowKeyPressed()
        {
            if (history.GetNext(out var command))
            {
                commandText.Replace(command);
                virtualCursor.Forward(command.Length);
            }
        }

        private void PageUpPressed()
        {
        }

        private void PageDownPressed()
        {
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
            if (!OperatingSystem.IsWindows()) return;

            var clipboardStr = WindowsClipboard.GetText();

            if (!string.IsNullOrEmpty(clipboardStr))
            {
                commandText.Replace(clipboardStr);
                virtualCursor.Forward(clipboardStr.Length);
            }
        }

        private void CharacterKeyPress(ConsoleKeyInfo key)
        {
            commandText.InsertCharacter(key.KeyChar);
            virtualCursor.Forward();
        }
    }
}
