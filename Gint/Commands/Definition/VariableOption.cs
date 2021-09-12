namespace Gint
{
    public class VariableOption : Option
    {
        public VariableOption(int priority, string argument, string longArgument, bool allowMultiple, ExecutionBlock callback, HelpCallback helpCallback)
            : base(priority, argument, longArgument, allowMultiple, callback, helpCallback)
        {
        }

    }
}
