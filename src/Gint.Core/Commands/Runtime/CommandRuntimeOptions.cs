using Gint.Markup;
using Gint.Pipes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gint
{
    public class CommandRuntimeOptions
    {
        public CommandRuntimeOptions(Func<IPipe> pipeFactory, Func<IPipe> initialPipe)
        {
            PipeFactory = pipeFactory;
            EntryPipe = initialPipe;
        }

        public bool LogParseTree { get; set; }
        public bool LogBindTree { get; set; }
        public bool AbortOnCancelKeyPress { get; set; }

        public Out RuntimeInfo { get; init; } = new Out();
        public Out InfoOut { get; init; } = new Out();
        public Out ErrorOut { get; init; } = new Out();

        public Func<IPipe> PipeFactory { get; init; }
        public Func<IPipe> EntryPipe { get; init; }

        public List<IEvaluationMiddleware> Middlewares { get; } = new List<IEvaluationMiddleware>();

        public void AddMiddleware(Func<CommandExecutionContext, Func<Task<CommandOutput>>, Task<CommandOutput>> middlewareDelegate)
        {
            Middlewares.Add(new EvaluationMiddlewareFromDelegate(middlewareDelegate));
        }

        public static CommandRuntimeOptions DefaultConsole =>
            new CommandRuntimeOptions(
                initialPipe: () => new GintPipe(new PipeOptions { PreferredBufferSegmentSize = 64 }),
                pipeFactory: () => new GintPipe(new PipeOptions { PreferredBufferSegmentSize = 64 }))
            {
                LogParseTree = false,
                LogBindTree = false,
                AbortOnCancelKeyPress = true,
                RuntimeInfo = Out.WithConsoleWriter,
                InfoOut = Out.WithConsoleWriter,
                ErrorOut = Out.WithConsoleWriter
            };
    }
}
