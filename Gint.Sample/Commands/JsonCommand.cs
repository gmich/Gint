using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gint.Sample
{
    internal sealed class JsonCommand : IScanForAttributes
    {
        [Command("json", helpCallback: nameof(JsonHelp))]
        public Task<ICommandOutput> Json(ICommandInput input, CommandExecutionContext ctx, Func<Task> next)
        {
            if (!input.Stream.IsEmpty)
            {
                var jsonToParse = input.Stream.Raw;
                try
                {
                    using (JsonDocument doc = JsonDocument.Parse(jsonToParse))
                    {
                        ctx.Info.WriteLine("Valid json");
                        if (input.Options.Contains("-p"))
                        {
                            var pretty = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
                            ctx.Info.WriteLine(pretty);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ctx.Error
                        .WriteFormatted($"Json reader exception", FormatType.RedForeground)
                        .WriteLine()
                        .Write($"JSON: {jsonToParse}")
                        .WriteLine();
                    ctx.Error.Write(ex.Message);
                    return CommandOutput.ErrorTask;
                }
            }
            else
            {
                ctx.Error.Write("No json in outstream");
                return CommandOutput.ErrorTask;
            }

            return CommandOutput.SuccessfulTask;
        }

        public static void JsonHelp(Out o)
        {
            o.Write("Json utilities.");
        }

        [Option(1, "-p", "--pretty", false, nameof(PrettyHelp))]
        private Task<ICommandOutput> Prettify(ICommandInput input, CommandExecutionContext ctx, Func<Task> next)
        {
            ctx.OutStream.Write(input.Stream.Raw);
            return CommandOutput.SuccessfulTask;
        }

        private static void PrettyHelp(Out @out)
        {
            @out.Write("Includes options details.");
        }
    }

}
