using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Gint.Markup;

namespace Gint
{
    internal class Evaluator
    {
        private CommandExecutionContext commandExecutionContext;
        private readonly ExecutionBuffer buffer;
        private readonly EvaluationChain evaluationChain = new EvaluationChain();

        private int executionId = 1;
        private int GetExecutionId => executionId++;
        private string[] boundOptionsCache = Array.Empty<string>();

        private Evaluator(CommandExecutionContext commandExecutionContext)
        {
            this.commandExecutionContext = commandExecutionContext;
            buffer = new ExecutionBuffer();
            commandExecutionContext.OutStream.AddWriter(new BufferOutputWriter(buffer));
        }

        public static async Task Evaluate(BoundNode root, string command, CommandExecutionContext commandExecutionContext)
        {
            var evaluator = new Evaluator(commandExecutionContext);
            evaluator.EvaluateNode(root, new CommandScope());

            try
            {
                await evaluator.Next.Invoke();
            }
            catch (Exception ex)
            {
                evaluator.commandExecutionContext.Info.Flush();
                evaluator.commandExecutionContext.Error.Flush();
                evaluator.commandExecutionContext.Error.WriteLine(ex.ToString());
                evaluator.commandExecutionContext.Error.Flush();
                evaluator.evaluationChain.Error = true;
            }
            finally
            {
                var stream = evaluator.GetStream();
                evaluator.commandExecutionContext.Info.WriteRaw(stream.Raw).WriteLine();
                evaluator.commandExecutionContext.Info.Flush();
                if (evaluator.evaluationChain.Error)
                {
                    evaluator.PrintError(command);
                }
            }
        }

        private void PrintError(string command)
        {
            var errorSpan = evaluationChain.Spans[evaluationChain.LastInvokedPosition];
            var error = command.Substring(errorSpan.Start, errorSpan.Length);
            var prefix = command.Substring(0, errorSpan.Start);
            var suffix = command[errorSpan.End..];

            commandExecutionContext.Error
                .Write("error: ")
                .Write(prefix)
                .WithForegroundColor().Red()
                .Write(error)
                .Write(suffix)
                .WriteLine();

            commandExecutionContext.Error.Flush();
        }

        private void EvaluateNode(BoundNode node, CommandScope scope)
        {
            switch (node.Kind)
            {
                case BoundNodeKind.Command:
                    EvaluateCommand((BoundCommand)node, scope);
                    break;
                case BoundNodeKind.CommandWithVariable:
                    EvaluateCommandWithVariable((BoundCommandWithVariable)node, scope);
                    break;
                case BoundNodeKind.Option:
                    EvaluateOption((BoundOption)node, scope);
                    break;
                case BoundNodeKind.VariableOption:
                    EvaluateVariableOption((BoundVariableOption)node, scope);
                    break;
                case BoundNodeKind.Pipe:
                    EvaluatePipe((BoundPipe)node, scope);
                    break;
                case BoundNodeKind.Pipeline:
                    EvaluatePipeline((BoundPipeline)node, scope);
                    break;
            }
        }

        private void EvaluatePipeline(BoundPipeline boundPipeline, CommandScope scope)
        {
            EvaluateNode(boundPipeline.FirstNode, scope);
            EvaluatePipe(boundPipeline.Pipe, scope);
            EvaluateNode(boundPipeline.SecondNode, scope);
        }

        private void EvaluatePipe(BoundPipe boundPipe, CommandScope scope)
        {

        }

        private void EvaluateVariableOption(BoundVariableOption boundVariableOption, CommandScope scope)
        {
            var capturedBoundOptions = BoundOptionsCopy;
            evaluationChain.Add(() =>
            {
                return boundVariableOption.VariableOption.Callback.Invoke(new CommandInput(GetExecutionId, boundVariableOption.Variable, GetStream(), capturedBoundOptions, scope), commandExecutionContext, Next)
                .ContinueWith(c => OnExecutionEnd(c.Result), TaskContinuationOptions.AttachedToParent);
            }, boundVariableOption.TextSpanWithVariable);
        }

        private void EvaluateOption(BoundOption boundOption, CommandScope scope)
        {
            var capturedBoundOptions = BoundOptionsCopy;

            evaluationChain.Add(() =>
            {
                return boundOption.Option.Callback.Invoke(new CommandInput(GetExecutionId, string.Empty, GetStream(), capturedBoundOptions, scope), commandExecutionContext, Next)
                .ContinueWith(c => OnExecutionEnd(c.Result), TaskContinuationOptions.AttachedToParent);
            }, boundOption.TextSpan);
        }

        private void EvaluateCommand(BoundCommand boundCommand, CommandScope scope)
        {
            EvaluateCommand(boundCommand, string.Empty, boundCommand.TextSpan, scope);
        }

        private void EvaluateCommandWithVariable(BoundCommandWithVariable boundCommand, CommandScope scope)
        {
            EvaluateCommand(boundCommand, boundCommand.Variable, boundCommand.TextSpanWithVariable, scope);
        }

        private void EvaluateCommand(BoundCommand boundCommand, string variable, TextSpan span, CommandScope scope)
        {
            var newScope = new CommandScope();
            boundOptionsCache = boundCommand.BoundOptions.Select(c => c.Argument).ToArray();
            var capturedBoundOptions = BoundOptionsCopy;

            foreach (var opt in boundCommand.BoundOptions)
            {
                EvaluateNode(opt, newScope);
            }

            evaluationChain.Add(() =>
            {
                return boundCommand.Command.Callback.Invoke(new CommandInput(GetExecutionId, variable, GetStream(), capturedBoundOptions, newScope), commandExecutionContext, Next)
                .ContinueWith(c => OnExecutionEnd(c.Result), TaskContinuationOptions.AttachedToParent);
            }, span);
        }


        private string[] BoundOptionsCopy => boundOptionsCache.ToArray();

        private Func<Task> Next => () =>
        {
            var captured = evaluationChain.Evaluated++;
            return evaluationChain.EvaluateNext(captured);
        };

        private void OnExecutionEnd(ICommandOutput output)
        {
            if (commandExecutionContext.CancellationToken.IsCancellationRequested)
            {
                evaluationChain.Error = true;
            }

            commandExecutionContext.Info.Flush();
            commandExecutionContext.Error.Flush();
            if (output.CommandState == CommandState.Error)
                evaluationChain.Error = true;
            Next();
        }

        private InputStream GetStream()
        {
            commandExecutionContext.OutStream.Flush();
            var stream = buffer.Drain();
            return stream;
        }
    }
}
