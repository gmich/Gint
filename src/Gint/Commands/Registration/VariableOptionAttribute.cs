using System;

namespace Gint
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class VariableOptionAttribute : Attribute
    {
        public VariableOptionAttribute(int priority, string argument, bool allowMultiple, string longArgument, string helpCallbackMethodName, string suggestionsCallback = null)
        {
            Priority = priority;
            Argument = argument;
            AllowMultiple = allowMultiple;
            LongArgument = longArgument;
            HelpCallbackMethodName = helpCallbackMethodName;
            SuggestionsCallback = suggestionsCallback;
        }

        public int Priority { get; }
        public string Argument { get; }
        public bool AllowMultiple { get; }
        public string LongArgument { get; }
        public string HelpCallbackMethodName { get; }
        public string SuggestionsCallback { get; }
    }
}
