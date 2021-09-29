﻿using Gint.Markup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gint
{

    public class CommandRuntime
    {
        public CommandRegistry CommandRegistry { get; }
        public CommandRuntimeOptions Options { get; } = new CommandRuntimeOptions();
        public Func<CommandExecutionContext> CommandExecutionContextFactory { get; set; }
        private OutTextWriterAdapter OutAdapter => new OutTextWriterAdapter(Options.Out);

        public event EventHandler<CommandExecutionEventArgs> OnCommandExecuting;
        public event EventHandler<CommandExecutionEventArgs> OnCommandExecuted;

        public CommandRuntime(CommandRegistry commandRegistry, Func<CommandExecutionContext> commandExecutionContextFactory)
        {
            CommandRegistry = commandRegistry;
            CommandExecutionContextFactory = commandExecutionContextFactory;

            AddShowTreesCommand();
        }

        private void AddShowTreesCommand()
        {
            CommandRegistry
            .AddCommand("showtrees", o => o.Write("Toggles showing of bind and parse trees."),
                (input, ctx, next) =>
                {
                    Options.LogBindTree = !Options.LogBindTree;
                    Options.LogParseTree = !Options.LogParseTree;

                    ctx.Info.Write("Showing bind / parse trees: ");
                    if (Options.LogBindTree)
                        ctx.Info.WithForegroundColor().Green().Write("✓");
                    else
                        ctx.Info.WithForegroundColor().Red().Write("x");

                    return CommandOutput.SuccessfulTask;
                });
        }

        public async Task Run(string command)
        {
            var parserRes = CommandExpressionTree.Parse(command);

            WriteDiagnostics(parserRes.Diagnostics, command);

            if (!parserRes.Diagnostics.Any())
            {
                if (Options.LogParseTree)
                {
                    Options.Out.WriteLine().WithForegroundColor().Magenta().Write("Parse tree").WriteLine();
                    parserRes.Root.WriteTo(OutAdapter);
                    Options.Out.WriteLine();
                    Options.Out.Flush();
                }

                var binder = new CommandBinder(parserRes.Root, CommandRegistry);
                var boundNode = binder.Bind();

                WriteDiagnostics(binder.Diagnostics, command);

                if (!binder.Diagnostics.Any())
                {
                    if (Options.LogBindTree)
                    {
                        Options.Out.WithForegroundColor().Magenta().Write("Bind tree")
                            .WriteLine();
                        boundNode.WriteTo(OutAdapter);
                        Options.Out.WriteLine();
                        Options.Out.Flush();
                    }
                    var ctx = CommandExecutionContextFactory();
                    OnCommandExecuting?.Invoke(this, new CommandExecutionEventArgs(ctx));
                    await Evaluator.Evaluate(boundNode, command, ctx);
                    OnCommandExecuted?.Invoke(this, new CommandExecutionEventArgs(ctx));
                }
            }
        }

        private void WriteDiagnostics(DiagnosticCollection diagnostics, string text)
        {
            foreach (var diagnostic in diagnostics)
            {
                var error = text.Substring(diagnostic.Location.Start, diagnostic.Location.Length);
                var prefix = text.Substring(0, diagnostic.Location.Start);
                var suffix = text[diagnostic.Location.End..];
                Options.Out.Write(prefix)
                    .WithForegroundColor().Red().Write(error)
                    .WriteLine(suffix.ToString())
                    .WriteLine(diagnostic.Message)
                    .WriteLine();

                Options.Out.Flush();
            }
        }
    }
}
