using Gint.Pipes;

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
                        input.Scope.PipeWriter.Write(input.Variable.ToUTF8EncodedByteArray());
                        return CommandOutput.SuccessfulTask;
                    });
        }
    }
}
