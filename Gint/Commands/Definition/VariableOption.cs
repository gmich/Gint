namespace Gint
{
    public class VariableOption : Option
    {
        public VariableOption(int priority, string argument, string longArgument, ExecutionBlock callback, HelpCallback helpCallback)
            : base(priority, argument, longArgument, callback, helpCallback)
        {
        }

    }
}
