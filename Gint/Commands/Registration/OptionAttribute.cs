using System;

namespace Gint
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class OptionAttribute : Attribute
    {
        public OptionAttribute(int priority, string argument, string longArgument, string helpCallbackMethodName)
        {
            Priority = priority;
            Argument = argument;
            LongArgument = longArgument;
            HelpCallbackMethodName = helpCallbackMethodName;
        }

        public int Priority { get; }
        public string Argument { get; }
        public string LongArgument { get; }
        public string HelpCallbackMethodName { get; }
    }
}
