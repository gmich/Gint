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
        private readonly VirtualCursor virtualCursor;
        private readonly ConsoleWindowResizer consoleWindowResizer;
        private readonly SystemConsoleAdapter systemConsoleAdapter;
        private readonly CommandText commandText;
        private readonly CommandPrinter commandPrinter;

        private int GetTotalSize() => commandText.Command.Length;

        public ConsoleInputManager()
        {
            prompt = new Prompt("cli>");
            virtualCursor = new VirtualCursor(GetTotalSize);
            consoleInputInterceptor = new ConsoleInputInterceptor();
            consoleWindowResizer = new ConsoleWindowResizer(virtualCursor);
            systemConsoleAdapter = new SystemConsoleAdapter(prompt, consoleWindowResizer, virtualCursor);
            commandText = new CommandText(virtualCursor);
            commandPrinter = new CommandPrinter();

            Setup();
        }

        public void Setup()
        {
            consoleWindowResizer.RecordCursorTop();
            prompt.Print();

            commandText.OnChange += (sender, args) =>
            {
                systemConsoleAdapter.ClearConsoleInput(args.Previous.Length);
                prompt.Print();
                commandPrinter.Print(commandText.Command);
            };

            virtualCursor.OnPositionChanged += (sender, args) =>
            {
                systemConsoleAdapter.AdjustToVirtualCursor();
            };
        }

        public void Run()
        {
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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

    internal class CommandPrinter
    {
        public void Print(string command)
        {
            Console.Write(command);
        }
    }

    internal interface IReadonlyVirtualCursor
    {
        public int Index { get; }
    }
    internal class VirtualCursor : IReadonlyVirtualCursor
    {
        private int _index;
        public int Index
        {
            get
            {
                return _index;
            }
            private set
            {
                var previous = _index;
                _index = value;

                if (previous != _index)
                {
                    OnPositionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler OnPositionChanged;
        public Func<int> RightBound;

        public VirtualCursor(Func<int> rightBound)
        {
            RightBound = rightBound;
        }

        private void MoveCursor(int steps)
        {
            var previous = Index;
            Index = Math.Clamp(Index + steps, 0, RightBound());
        }

        public void Forward(int steps = 1)
        {
            MoveCursor(steps);
        }

        public void Back(int steps = 1)
        {
            MoveCursor(-steps);
        }

        public void Reset()
        {
            Index = 0;
        }
    }

    internal class ConsoleInputInterceptor
    {
        public ConsoleKeyInfo GetNextKey()
        {
            Console.CursorVisible = true;
            var key = Console.ReadKey(intercept: true);
            Console.CursorVisible = false;
            return key;
        }
    }

    internal class CommandText
    {
        private readonly IReadonlyVirtualCursor virtualCursor;

        public CommandText(IReadonlyVirtualCursor virtualCursor)
        {
            this.virtualCursor = virtualCursor;
            Command = string.Empty;
        }

        public string Command { get; private set; }

        public event EventHandler<CommandTextChangedEventArgs> OnChange;

        private void RaiseOnChangeEvent(string previous)
        {
            OnChange?.Invoke(this, new CommandTextChangedEventArgs(previous, Command));
        }

        public void Clear()
        {
            var previous = Command;
            Command = string.Empty;
            RaiseOnChangeEvent(previous);
        }

        public void InsertCharacter(char c)
        {
            var previous = Command;
            Command = $"{string.Concat(Command.Take(virtualCursor.Index))}{c}{string.Concat(Command.Skip(virtualCursor.Index))}";
            RaiseOnChangeEvent(previous);
        }

        public void RemoveCurrentCharacter()
        {
            var previous = Command;
            Command = $"{string.Concat(Command.Take(virtualCursor.Index - 1))}{string.Concat(Command.Skip(virtualCursor.Index))}";
            RaiseOnChangeEvent(previous);
        }

        public void RemoveNextCharacter()
        {
            var previous = Command;
            Command = $"{string.Concat(Command.Take(virtualCursor.Index))}{string.Concat(Command.Skip(virtualCursor.Index + 1))}";
            RaiseOnChangeEvent(previous);
        }
    }
    internal class ConsoleWindowResizer
    {
        private readonly IReadonlyVirtualCursor virtualCursor;

        private int beforeReadKeyTop;
        private int beforeReadKeyBufferWidth;
        private int linesBeforeRead;
        private bool cursorTopRecorded = false;

        public int InputCursorTop { get; private set; }

        public ConsoleWindowResizer(IReadonlyVirtualCursor virtualCursor)
        {
            this.virtualCursor = virtualCursor;
        }

        public void RecordCursorTop()
        {
            if (cursorTopRecorded) return;
            InputCursorTop = Console.CursorTop;
        }

        public void Reset()
        {
            cursorTopRecorded = false;
        }

        public void RecordBufferState()
        {
            //get buffer and console top location
            beforeReadKeyTop = Console.CursorTop;
            beforeReadKeyBufferWidth = Console.BufferWidth;
            linesBeforeRead = virtualCursor.Index / Console.BufferWidth;
        }

        public void AdjustIfNeeded()
        {
            //compare and reset
            if (beforeReadKeyBufferWidth != Console.BufferWidth)
            {
                var linesAfterRead = virtualCursor.Index / Console.BufferWidth;
                var topDifference = (Console.CursorTop - beforeReadKeyTop);
                if (linesAfterRead != linesBeforeRead)
                {
                    topDifference += (linesBeforeRead - linesAfterRead);
                }
                InputCursorTop += topDifference;
            }
        }
    }

    internal class Prompt
    {
        public Prompt(string text)
        {
            Text = text;
        }

        public string Text { get; }
        public int Length => Text.Length;

        public void Print()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(Text);
            Console.ResetColor();
        }
    }
    internal class SystemConsoleAdapter
    {
        private readonly Prompt prompt;
        private readonly ConsoleWindowResizer consoleWindowResizer;
        private readonly IReadonlyVirtualCursor virtualCursor;

        public SystemConsoleAdapter(Prompt prompt, ConsoleWindowResizer consoleWindowResizer, IReadonlyVirtualCursor virtualCursor)
        {
            this.prompt = prompt;
            this.consoleWindowResizer = consoleWindowResizer;
            this.virtualCursor = virtualCursor;
        }

        public event EventHandler OnInputCleared;

        public void ClearConsoleInput(int characters)
        {
            var totalLines = (characters) / Console.BufferWidth;
            totalLines += 1;

            var total = (totalLines) * (Console.WindowWidth - 1);
            SetConsoleCursorToInputStart();

            var cleanup = new string(' ', total);
            Console.Write(cleanup);
            SetConsoleCursorToInputStart();
            OnInputCleared?.Invoke(this, EventArgs.Empty);
        }

        private int VirtualCursorWithPromptPosition => virtualCursor.Index + prompt.Length;

        public void AdjustToVirtualCursor()
        {
            int line = (VirtualCursorWithPromptPosition / Console.BufferWidth);

            var shouldBe = consoleWindowResizer.InputCursorTop + line;
            Console.CursorTop = shouldBe;
            Console.CursorLeft = VirtualCursorWithPromptPosition % Console.BufferWidth;
        }

        public void SetConsoleCursorToInputStart()
        {
            Console.CursorLeft = 0;
            Console.CursorTop = consoleWindowResizer.InputCursorTop;
        }
    }
}
