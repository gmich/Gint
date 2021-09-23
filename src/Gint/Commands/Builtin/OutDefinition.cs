namespace Gint.Builtin
{
    internal sealed class OutDefinition : ICommandDefinition
    {
        public void Register(CommandRegistry registry)
        {
            registry
                .AddVariableCommand("out", required: true, helpCallback: o => o.Write("Prints to stream out"),
                    (input, ctx, next) =>
                    {
                        ctx.OutStream.Write(input.Variable);
                        return CommandOutput.SuccessfulTask;
                    });
        }
    }
}
