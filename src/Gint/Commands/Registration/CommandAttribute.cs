using System;

namespace Gint
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string name, string helpCallback, string suggestionsCallback = null)
        {
            CommandName = name;
            HelpCallbackMethodName = helpCallback;
            SuggestionsCallback = suggestionsCallback;
        }

        public string CommandName { get; }
        public string HelpCallbackMethodName { get; }
        public string SuggestionsCallback { get; }
    }


}
