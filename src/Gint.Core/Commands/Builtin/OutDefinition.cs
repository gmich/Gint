using Gint.Pipes;

namespace Gint.Builtin
{
    internal sealed class OutDefinition : ICommandDefinition
    {
        public void Register(CommandRegistry registry)
        {
            registry
                .AddVariableCommand("out", required: true, helpCallback: o => o.Write("Prints to stream out"),
                    (ctx, next) =>
                    {
                        ctx.Scope.PipeWriter.Write(ctx.ExecutingCommand.Variable.ToUTF8EncodedByteArray());
                        return CommandOutput.SuccessfulTask;
                    });
        }
    }
}
