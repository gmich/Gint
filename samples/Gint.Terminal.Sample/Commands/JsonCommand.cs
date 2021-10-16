using Gint.Markup;
using Gint.Pipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gint.Terminal.Sample
{
    internal sealed class JsonCommand : IScanForAttributes
    {
        [CommandWithVariable("json", required: false, helpCallback: nameof(JsonHelp), suggestionsCallback: nameof(JsonSuggestions))]
        public Task<CommandOutput> Json(CommandExecutionContext ctx)
        {
            var inputVar = ctx.ExecutingCommand.HasVariable ? ctx.ExecutingCommand.Variable : ctx.Scope.ReadInputAsString();
            if (!string.IsNullOrEmpty(inputVar))
            {
                var jsonToParse = inputVar;
                try
                {
                    using (JsonDocument doc = JsonDocument.Parse(jsonToParse))
                    {
                        ctx.Info.WriteLine("Valid json");
                        if (ctx.ExecutingCommand.Options.Contains("-p"))
                        {
                            var pretty = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
                            ctx.Info.WriteLine(pretty);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ctx.Error
                        .WithForegroundColor().Red()
                        .Write($"Json reader exception")
                        .WriteLine()
                        .Write($"JSON: {jsonToParse}")
                        .WriteLine();
                    ctx.Error.Write(ex.Message);
                    return CommandOutput.ErrorTask;
                }
            }
            else
            {
                ctx.Error.WriteLine("No json in outstream");
                return CommandOutput.ErrorTask;
            }

            return CommandOutput.SuccessfulTask;
        }

        public static IEnumerable<Suggestion> JsonSuggestions(string variable)
        {
            if (!string.IsNullOrEmpty(variable)) return Enumerable.Empty<Suggestion>();

            return new[]
            {
               new Suggestion(value: "\'{ \"name\":\"kokos\" , \"age\":18 }\'", displayValue: "random_json")
            };
        }

        public static void JsonHelp(Out o)
        {
            o.Write("Json utilities.");
        }

        [Option(1, "-p", "--pretty", false, nameof(PrettyHelp))]
        private Task<CommandOutput> Prettify(CommandExecutionContext ctx)
        {
            return CommandOutput.SuccessfulTask;
        }

        private static void PrettyHelp(Out @out)
        {
            @out.Write("Includes options details.");
        }
    }

}
