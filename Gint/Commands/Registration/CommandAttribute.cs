using System;

namespace Gint
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string name, string helpCallback)
        {
            CommandName = name;
            HelpCallbackMethodName = helpCallback;
        }

        public string CommandName { get; }
        public string HelpCallbackMethodName { get; }
    }


}
