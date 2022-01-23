# Gint

![.Net5 Build](https://github.com/gmich/gint/actions/workflows/dotnet.yml/badge.svg) 

A powerful and configurable command line interpreter for .NET with no dependencies.


## Quickstart

![Markup tldr](https://github.com/gmich/Gint/blob/main/resources/gint-quickstart.gif)

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
                    var name = ctx.Scope.GetValueOrDefault(key: "--name", @default: "Gint");

                    var txt = ctx.Formatter
                    .WithBackgroundColor().White()
                    .AndForeground().Black()
                    .Write($"Hello {name}!");

                    ctx.Scope.WriteString(txt);

                    return CommandResult.SuccessfulTask;
                })
                .AddVariableOption(
                    argument: "-n",
                    longArgument: "--name",
                    helpCallback: o => o.WithForegroundColor().Cyan().Write("Give a name!"),
                    callback: ctx =>
                    {
                        ctx.Scope.Add("--name", ctx.ExecutingCommand.Variable);
                        return CommandResult.SuccessfulTask;
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

## Tables

![Simple table](https://github.com/gmich/Gint/blob/main/resources/simple_table_with_defaults.png)

```csharp
            GintTable
                .WithFirstRowAsHeader()
                    .AddColumn("header1")
                    .AddColumn("header2")
                 .NewRow()
                     .AddColumn("column1")
                     .AddColumn("column2")
                 .NewRow()
                     .AddColumn("column1")
                     .AddColumn("column2")
                 .Build()
                 .RenderToConsole();
```  

For more info on tables, check the [wiki](https://github.com/gmich/Gint/wiki/Gint-tables).

