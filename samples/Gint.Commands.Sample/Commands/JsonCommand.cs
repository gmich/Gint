using Gint.Markup;
using Gint.Pipes;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gint.Sample
{
    internal sealed class JsonCommand : IScanForAttributes
    {
        [Command("json", helpCallback: nameof(JsonHelp))]
        public Task<CommandResult> Json(CommandExecutionContext ctx)
        {
            var inputStream = ctx.Scope.ReadInputAsString();
            if (!string.IsNullOrEmpty(inputStream))
            {
                var jsonToParse = inputStream;
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
                    return CommandResult.ErrorTask;
                }
            }
            else
            {
                ctx.Error.Write("No json in outstream");
                return CommandResult.ErrorTask;
            }

            return CommandResult.SuccessfulTask;
        }

        public static void JsonHelp(Out o)
        {
            o.Write("Json utilities.");
        }

        [Option(1, "-p", "--pretty", false, nameof(PrettyHelp))]
        private Task<CommandResult> Prettify(CommandExecutionContext ctx)
        {
            return CommandResult.SuccessfulTask;
        }

        private static void PrettyHelp(Out @out)
        {
            @out.Write("Includes options details.");
        }
    }

}
