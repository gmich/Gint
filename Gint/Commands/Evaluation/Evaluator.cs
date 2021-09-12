using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Gint
{
    internal class Evaluator
    {
        private readonly CommandExecutionContext commandExecutionContext;
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
            evaluator.EvaluateNode(root);

            try
            {
                await evaluator.Next.Invoke();
            }
            catch (Exception ex)
            {
                evaluator.commandExecutionContext.Info.Flush();
                evaluator.commandExecutionContext.Error.Flush();
                evaluator.commandExecutionContext.Error.WriteLine(ex.ToString());
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
                .WriteFormatted(error, FormatType.RedForeground)
                .Write(suffix)
                .WriteLine()
                .Flush();
        }

        private void EvaluateNode(BoundNode node)
        {
            switch (node.Kind)
            {
                case BoundNodeKind.Command:
                    EvaluateCommand((BoundCommand)node);
                    break;
                case BoundNodeKind.Option:
                    EvaluateOption((BoundOption)node);
                    break;
                case BoundNodeKind.VariableOption:
                    EvaluateVariableOption((BoundVariableOption)node);
                    break;
                case BoundNodeKind.Pipe:
                    EvaluatePipe((BoundPipe)node);
                    break;
                case BoundNodeKind.Pipeline:
                    EvaluatePipeline((BoundPipeline)node);
                    break;
            }
        }

        private void EvaluatePipeline(BoundPipeline boundPipeline)
        {
            EvaluateNode(boundPipeline.FirstNode);
            EvaluatePipe(boundPipeline.Pipe);
            EvaluateNode(boundPipeline.SecondNode);
        }

        private void EvaluatePipe(BoundPipe boundPipe)
        {

        }

        private void EvaluateVariableOption(BoundVariableOption boundVariableOption)
        {
            var capturedBoundOptions = BoundOptionsCopy;
            evaluationChain.Add(async () =>
            {
                var output = await boundVariableOption.VariableOption.Callback.Invoke(new CommandInput(GetExecutionId, boundVariableOption.Variable, GetStream(), capturedBoundOptions), commandExecutionContext, Next);
                OnExecutionEnd(output);
            }, boundVariableOption.TextSpanWithVariable);
        }

        private void EvaluateOption(BoundOption boundOption)
        {
            var capturedBoundOptions = BoundOptionsCopy;
            evaluationChain.Add(async () =>
            {
                var output = await boundOption.Option.Callback.Invoke(new CommandInput(GetExecutionId, string.Empty, GetStream(), capturedBoundOptions), commandExecutionContext, Next);
                OnExecutionEnd(output);
            }, boundOption.TextSpan);
        }

        private void EvaluateCommand(BoundCommand boundCommand)
        {
            boundOptionsCache = boundCommand.BoundOptions.Select(c => c.Argument).ToArray();
            var capturedBoundOptions = BoundOptionsCopy;
            evaluationChain.Add(async () =>
            {
                var output = await boundCommand.Command.Callback.Invoke(new CommandInput(GetExecutionId, string.Empty, GetStream(), capturedBoundOptions), commandExecutionContext, Next);
                OnExecutionEnd(output);
            }, boundCommand.TextSpan);

            foreach (var opt in boundCommand.BoundOptions)
            {
                EvaluateNode(opt);
            }
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
