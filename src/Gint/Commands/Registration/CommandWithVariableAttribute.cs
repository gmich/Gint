using System;

namespace Gint
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandWithVariableAttribute : Attribute
    {
        public CommandWithVariableAttribute(string name, bool required, string helpCallback)
        {
            CommandName = name;
            Required = required;
            HelpCallbackMethodName = helpCallback;
        }

        public string CommandName { get; }
        public bool Required { get; }
        public string HelpCallbackMethodName { get; }
    }


}
