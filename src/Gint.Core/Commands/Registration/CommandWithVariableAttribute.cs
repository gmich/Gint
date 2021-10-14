using System;

namespace Gint
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandWithVariableAttribute : Attribute
    {
        public CommandWithVariableAttribute(string name, bool required, string helpCallback, string suggestionsCallback = null)
        {
            CommandName = name;
            Required = required;
            HelpCallbackMethodName = helpCallback;
            SuggestionsCallback = suggestionsCallback;
        }

        public string CommandName { get; }
        public bool Required { get; }
        public string HelpCallbackMethodName { get; }
        public string SuggestionsCallback { get; }
    }


}
