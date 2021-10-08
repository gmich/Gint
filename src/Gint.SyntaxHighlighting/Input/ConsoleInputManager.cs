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
        private ConsoleVirtualBufferHandler consoleVirtualBuffer;
        private SystemConsoleAdapter systemConsoleAdapter;
        private CommandText commandText;
        private SuggestionRenderer suggestionRenderer;

        public event EventHandler<string> OnCommandReady;

        private int GetTotalSize() => commandText.Value.Length;
        private bool requiresReset = true;

        public bool AcceptInput { get; set; } = true;

        public ConsoleInputManager(CommandRegistry registry = null)
        {
            prompt = new Prompt("cli> ");
            consoleInputInterceptor = new ConsoleInputInterceptor();
            renderer = new CommandRenderer()
            {
                DisplayErrorCells = false,
                DisplayDiagnostics = true,
                Registry = registry ?? CommandRegistry.Empty
            };
            suggestionRenderer = new SuggestionRenderer();
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
            commandText = new CommandText(virtualCursor);
            consoleVirtualBuffer = new ConsoleVirtualBufferHandler(virtualCursor, commandText);
            systemConsoleAdapter = new SystemConsoleAdapter(prompt, consoleVirtualBuffer, virtualCursor);
        }

        private void Reset()
        {
            CreateActors();

            commandText.OnChange += (sender, args) =>
            {
                var renderCallback = renderer.GenerateRenderCallback(commandText.Value);
                systemConsoleAdapter.ClearConsoleInput(consoleVirtualBuffer.GetTotalCharactersInVirtualBuffer());
                prompt.Print();
                renderCallback();
                consoleVirtualBuffer.RecordLastCursorLine();
                systemConsoleAdapter.AdjustToVirtualCursor();
            };

            ResetSuggestions();

            virtualCursor.OnPositionChanged += (sender, args) =>
            {
                systemConsoleAdapter.AdjustToVirtualCursor();
            };

            commandText.Clear();

            requiresReset = false;
        }

        private void ResetSuggestions()
        {
            suggestionRenderer.OnLostFocus += (sender, args) =>
            {
                AcceptInput = false;
                if (args.SuggestionAccepted)
                    commandText.AddSuffix(args.Value);
                else
                    commandText.Replace(commandText.Value);
                virtualCursor.Forward(GetTotalSize());
                AcceptInput = true;
            };

            suggestionRenderer.OnChange += (sender, args) =>
            {
                var renderCallback = renderer.GenerateRenderCallback(commandText.Value);
                systemConsoleAdapter.ClearConsoleInput(consoleVirtualBuffer.GetTotalCharactersInVirtualBuffer());
                prompt.Print();
                renderCallback();
                consoleVirtualBuffer.RecordLastCursorLine();
                systemConsoleAdapter.AdjustToVirtualCursor();
                suggestionRenderer.Render();
            };
        }


        public void WaitNext()
        {
            if (!AcceptInput) return;

            if (requiresReset) Reset();

            consoleVirtualBuffer.RecordBufferState();
            var key = consoleInputInterceptor.GetNextKey();
            consoleVirtualBuffer.AdjustIfNeeded();

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

            systemConsoleAdapter.NewLine();
            consoleVirtualBuffer.RecordInputTop();
            systemConsoleAdapter.ClearConsoleInput(consoleVirtualBuffer.GetTotalCharactersInVirtualBuffer());
            OnCommandReady?.Invoke(this, commandText.Value);
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
            suggestionRenderer.Init();
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
