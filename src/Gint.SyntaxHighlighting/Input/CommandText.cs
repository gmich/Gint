using System;
using System.Linq;

namespace Gint.SyntaxHighlighting
{
    internal class CommandText
    {
        private readonly IReadonlyVirtualCursor virtualCursor;

        public CommandText(IReadonlyVirtualCursor virtualCursor)
        {
            this.virtualCursor = virtualCursor;
            Value = string.Empty;
        }

        public string Value { get; private set; }

        public override string ToString() => Value;

        public event EventHandler<CommandTextChangedEventArgs> OnChange;

        private void RaiseOnChangeEvent(string previous)
        {
            OnChange?.Invoke(this, new CommandTextChangedEventArgs(previous, Value));
        }

        public void Clear()
        {
            var previous = Value;
            Value = string.Empty;
            RaiseOnChangeEvent(previous);
        }

        public bool IsLastCharacterWhitespace()
        {
            if (string.IsNullOrEmpty(Value)) return true;

            return char.IsWhiteSpace(Value[^1]);
        }

        public void Replace(string newCommand)
        {
            var previous = Value;
            Value = newCommand;
            RaiseOnChangeEvent(previous);
        }

        public void AddSuffix(string suffix)
        {
            var previous = Value;
            Value += suffix;
            RaiseOnChangeEvent(previous);
        }

        public void InsertCharacter(char c)
        {
            var previous = Value;
            Value = $"{string.Concat(Value.Take(virtualCursor.Index))}{c}{string.Concat(Value.Skip(virtualCursor.Index))}";
            RaiseOnChangeEvent(previous);
        }

        public void RemoveCurrentCharacter()
        {
            var previous = Value;
            Value = $"{string.Concat(Value.Take(virtualCursor.Index - 1))}{string.Concat(Value.Skip(virtualCursor.Index))}";
            RaiseOnChangeEvent(previous);
        }

        public void RemoveNextCharacter()
        {
            var previous = Value;
            Value = $"{string.Concat(Value.Take(virtualCursor.Index))}{string.Concat(Value.Skip(virtualCursor.Index + 1))}";
            RaiseOnChangeEvent(previous);
        }
    }
}
