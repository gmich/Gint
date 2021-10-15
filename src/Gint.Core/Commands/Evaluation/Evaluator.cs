using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Gint.Markup;
using System.Collections;
using System.Collections.Generic;
using Gint.Pipes;

namespace Gint
{

    internal class Pipeline
    {
        public CommandScope PreviousScope { get; init; }
        public IPipe PreviousPipe { get; init; }
        public CommandScope Scope { get; init; }
        public IPipe Pipe { get; init; }
    }

    internal class Evaluator
    {
        private readonly EvaluationChain evaluationChain = new EvaluationChain();
        private readonly Stack<Pipeline> pipelines = new Stack<Pipeline>();

        private CommandExecutionContext commandExecutionContext;
        private readonly Func<IPipe> entryPipe;
        private readonly Func<IPipe> pipeFactory;
        private int executionId = 1;
        private int GetExecutionId => executionId++;
        private string[] boundOptionsCache = Array.Empty<string>();

        private Evaluator(CommandExecutionContext commandExecutionContext, Func<IPipe> entryPipe, Func<IPipe> pipeFactory)
        {
            this.commandExecutionContext = commandExecutionContext;
            this.entryPipe = entryPipe;
            this.pipeFactory = pipeFactory;
            SeedFirstPipeline();
        }

        private void SeedFirstPipeline()
        {
            if (pipelines.Count > 0) return;

            var inputPipe = entryPipe();
            var firstPipe = pipeFactory();
            pipelines.Push(new Pipeline
            {
                PreviousScope = new CommandScope(inputPipe.Writer, inputPipe.Reader),
                PreviousPipe = inputPipe,
                Scope = new CommandScope(firstPipe.Writer, firstPipe.Reader),
                Pipe = firstPipe
            });
        }

        private Pipeline GetLastPipeline()
        {
            return pipelines.Peek();
        }


        public static async Task Evaluate(BoundNode root, string command, CommandExecutionContext commandExecutionContext, Func<IPipe> entryPipe, Func<IPipe> pipeFactory)
        {
            var evaluator = new Evaluator(commandExecutionContext, entryPipe, pipeFactory);
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
                evaluator.commandExecutionContext.Error.Flush();
                evaluator.evaluationChain.Error = true;
            }
            finally
            {
                var lastPipeline = evaluator.GetLastPipeline();
                var stream = lastPipeline.Pipe.Read().Buffer;
                var parsedStream = stream?.ToUTF8String() ?? string.Empty;
                if (parsedStream != string.Empty)
                {
                    evaluator.commandExecutionContext.Info.WriteRaw(parsedStream).WriteLine();
                    evaluator.commandExecutionContext.Info.Flush();
                }
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

        private void EvaluateNode(BoundNode node)
        {
            switch (node.Kind)
            {
                case BoundNodeKind.Command:
                    EvaluateCommand((BoundCommand)node);
                    break;
                case BoundNodeKind.CommandWithVariable:
                    EvaluateCommandWithVariable((BoundCommandWithVariable)node);
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
            var lastPipeline = GetLastPipeline();
            var newPipe = pipeFactory();
            var pipeline = new Pipeline
            {
                PreviousScope = lastPipeline.Scope,
                PreviousPipe = lastPipeline.Pipe,
                Pipe = newPipe,
                Scope = new CommandScope(newPipe.Writer, lastPipeline.Pipe.Reader)
            };
            pipelines.Push(pipeline);
        }

        private void EvaluateVariableOption(BoundVariableOption boundVariableOption)
        {
            var capturedBoundOptions = BoundOptionsCopy;
            var scope = GetLastPipeline().Scope;
            evaluationChain.Add(() =>
            {
                return boundVariableOption.VariableOption.Callback.Invoke(new CommandInput(GetExecutionId, boundVariableOption.Variable, capturedBoundOptions, scope), commandExecutionContext, Next)
                .ContinueWith(c => OnExecutionEnd(c.Result), TaskContinuationOptions.AttachedToParent);
            }, boundVariableOption.TextSpanWithVariable);
        }

        private void EvaluateOption(BoundOption boundOption)
        {
            var capturedBoundOptions = BoundOptionsCopy;
            var scope = GetLastPipeline().Scope;
            evaluationChain.Add(() =>
            {
                return boundOption.Option.Callback.Invoke(new CommandInput(GetExecutionId, string.Empty, capturedBoundOptions, scope), commandExecutionContext, Next)
                .ContinueWith(c => OnExecutionEnd(c.Result), TaskContinuationOptions.AttachedToParent);
            }, boundOption.TextSpan);
        }

        private void EvaluateCommand(BoundCommand boundCommand)
        {
            EvaluateCommand(boundCommand, string.Empty, boundCommand.TextSpan);
        }

        private void EvaluateCommandWithVariable(BoundCommandWithVariable boundCommand)
        {
            EvaluateCommand(boundCommand, boundCommand.Variable, boundCommand.TextSpanWithVariable);
        }

        private void EvaluateCommand(BoundCommand boundCommand, string variable, TextSpan span)
        {
            var newScope = GetLastPipeline().Scope;
            boundOptionsCache = boundCommand.BoundOptions.Select(c => c.Argument).ToArray();
            var capturedBoundOptions = BoundOptionsCopy;

            foreach (var opt in boundCommand.BoundOptions)
            {
                EvaluateNode(opt);
            }

            evaluationChain.Add(() =>
            {
                return boundCommand.Command.Callback.Invoke(new CommandInput(GetExecutionId, variable, capturedBoundOptions, newScope), commandExecutionContext, Next)
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
    }
}
