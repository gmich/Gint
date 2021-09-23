using System;

namespace Gint
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class VariableOptionAttribute : Attribute
    {
        public VariableOptionAttribute(int priority, string argument, bool allowMultiple, string longArgument, string helpCallbackMethodName)
        {
            Priority = priority;
            Argument = argument;
            AllowMultiple = allowMultiple;
            LongArgument = longArgument;
            HelpCallbackMethodName = helpCallbackMethodName;
        }

        public int Priority { get; }
        public string Argument { get; }
        public bool AllowMultiple { get; }
        public string LongArgument { get; }
        public string HelpCallbackMethodName { get; }
    }
}
