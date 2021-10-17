# Gint

![.Net5 Build](https://github.com/gmich/gint/actions/workflows/dotnet.yml/badge.svg) 

A powerful and configurable command line interpreter.


## Quickstart

```csharp
    class Program
    {
        static void Main(string[] args)
        {
            var runtime = new CommandRuntime();

            runtime.CommandRegistry.AddCommand(
                commandName: "hello",
                helpCallback: o => o.WithForegroundColor().Cyan().Write("help!"),
                callback: ctx =>
                {
                    var name = ctx.Scope.GetOrDefault("--name", "Gint");

                    var txt = ctx.Formatter
                    .WithBackgroundColor().White()
                    .AndForeground().Black()
                    .Write($"Hello {name}!");

                    ctx.Scope.WriteString(txt);

                    return CommandOutput.SuccessfulTask;
                })
                .AddVariableOption(
                    argument: "-n",
                    longArgument: "--name",
                    helpCallback: o => o.WithForegroundColor().Cyan().Write("Give a name!"),
                    callback: ctx =>
                    {
                        ctx.Scope.Add("--name", ctx.ExecutingCommand.Variable);
                        return CommandOutput.SuccessfulTask;
                    },
                    suggestions: v => new Suggestion[] { "Teresa", "Devin", "Michael", "Maria", "George" });

            var terminal = new CommandTerminal(runtime);

            while (true)
            {
                terminal.WaitForInput();
            }
        }
    }
```

![Markup tldr](https://github.com/gmich/Gint/blob/main/resources/gint-quickstart.gif)
