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

    internal class Evaluator
    {
        private readonly EvaluationChain evaluationChain = new EvaluationChain();
        private readonly Stack<Pipeline> pipelines = new Stack<Pipeline>();
        private readonly string command;
        private GlobalExecutionContext globalExecutionContext;
        private readonly Func<IPipe> entryPipe;
        private readonly Func<IPipe> pipeFactory;
        private readonly IEvaluationMiddleware[] middlewares;
        private int executionId = 1;
        private int GetExecutionId => executionId++;
        private string[] boundOptionsCache = Array.Empty<string>();

        private Evaluator(string command, GlobalExecutionContext globalExecutionContext, Func<IPipe> entryPipe, Func<IPipe> pipeFactory, IEnumerable<IEvaluationMiddleware> middlewares)
        {
            this.command = command;
            this.globalExecutionContext = globalExecutionContext;
            this.entryPipe = entryPipe;
            this.pipeFactory = pipeFactory;
            this.middlewares = middlewares.ToArray();
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


        public static async Task Evaluate(BoundNode root, string command, GlobalExecutionContext globalExecutionContext, Func<IPipe> entryPipe, Func<IPipe> pipeFactory, IEnumerable<IEvaluationMiddleware> middlewares)
        {
            var evaluator = new Evaluator(command, globalExecutionContext, entryPipe, pipeFactory, middlewares);
            evaluator.EvaluateNode(root);

            try
            {
                await evaluator.Next.Invoke();
            }
            catch (Exception ex)
            {
                evaluator.globalExecutionContext.Info.Flush();
                evaluator.globalExecutionContext.Error.Flush();
                evaluator.globalExecutionContext.Error.WriteLine(ex.ToString());
                evaluator.globalExecutionContext.Error.Flush();
                evaluator.evaluationChain.Error = true;
            }
            finally
            {
                var lastPipeline = evaluator.GetLastPipeline();
                var stream = lastPipeline.Pipe.Read().Buffer;
                var parsedStream = stream?.ToUTF8String() ?? string.Empty;
                evaluator.globalExecutionContext.Info.WriteRaw(parsedStream).WriteLine();
                evaluator.globalExecutionContext.Info.Flush();
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

            globalExecutionContext.Error
                .Write("error: ")
                .Write(prefix)
                .WithForegroundColor().Red()
                .Write(error)
                .Write(suffix)
                .WriteLine();

            globalExecutionContext.Error.Flush();
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

        private CommandExecutionContext GetExecutionContext(ExecutingCommand commandInfo, CommandScope scope)
        {
            return new CommandExecutionContext(commandInfo, scope, globalExecutionContext, Next);
        }

        private void AddEvaluationChain(ExecutionBlock block, CommandExecutionContext ctx, TextSpan span)
        {
            evaluationChain.Add(() =>
            {
                return BuildExecutionBlockWithMiddlewares(0, block, ctx).Invoke()
                .ContinueWith(c => OnExecutionEnd(c.Result), TaskContinuationOptions.AttachedToParent);
            }, span);
        }

        private Func<Task<CommandOutput>> BuildExecutionBlockWithMiddlewares(int index, ExecutionBlock block, CommandExecutionContext ctx)
        {
            if (index == middlewares.Length)
            {
                return () => block.Invoke(ctx);
            }
            return () => middlewares[index].Intercept(ctx, BuildExecutionBlockWithMiddlewares(index + 1, block, ctx));
        }

        private void EvaluateVariableOption(BoundVariableOption boundVariableOption)
        {
            var capturedBoundOptions = BoundOptionsCopy;
            var scope = GetLastPipeline().Scope;
            var span = boundVariableOption.TextSpanWithVariable;
            var ctx = GetExecutionContext(new ExecutingCommand(GetExecutionId, command, boundVariableOption.Variable, capturedBoundOptions, span), scope);

            AddEvaluationChain(boundVariableOption.VariableOption.Callback, ctx, span);
        }


        private void EvaluateOption(BoundOption boundOption)
        {
            var capturedBoundOptions = BoundOptionsCopy;
            var scope = GetLastPipeline().Scope;
            var span = boundOption.TextSpan;
            var ctx = GetExecutionContext(new ExecutingCommand(GetExecutionId, command, string.Empty, capturedBoundOptions, span), scope);

            AddEvaluationChain(boundOption.Option.Callback, ctx, span);
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

            var ctx = GetExecutionContext(new ExecutingCommand(GetExecutionId, command, variable, capturedBoundOptions, span), newScope);

            AddEvaluationChain(boundCommand.Command.Callback, ctx, span);
        }


        private string[] BoundOptionsCopy => boundOptionsCache.ToArray();

        private Func<Task> Next => () =>
        {
            var captured = evaluationChain.Evaluated++;
            return evaluationChain.EvaluateNext(captured);
        };

        private void OnExecutionEnd(CommandOutput output)
        {
            if (globalExecutionContext.CancellationToken.IsCancellationRequested)
            {
                evaluationChain.Error = true;
            }

            globalExecutionContext.Info.Flush();
            globalExecutionContext.Error.Flush();
            if (output.CommandState == CommandState.Error)
                evaluationChain.Error = true;
            Next();
        }
    }
}
