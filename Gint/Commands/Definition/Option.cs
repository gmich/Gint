﻿namespace Gint
{
    public class Option
    {
        public Option(int priority, string argument, string longArgument, ExecutionBlock callback, HelpCallback helpCallback)
        {
            Priority = priority;
            Argument = argument;
            LongArgument = longArgument;
            Callback = callback;
            HelpCallback = helpCallback;
        }

        public int Priority { get; }
        public string Argument { get; }
        public string LongArgument { get; }

        public ExecutionBlock Callback { get; }
        public HelpCallback HelpCallback { get; }

        public override string ToString() => $"[{Argument} , {LongArgument}]";
    }
}
