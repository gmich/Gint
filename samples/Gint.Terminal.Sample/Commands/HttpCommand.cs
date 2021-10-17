using Gint.Markup;
using Gint.Pipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gint.Terminal.Sample
{
    internal sealed class HttpCommand : IScanForAttributes
    {
        private static WebClient client = new WebClient();

        [CommandWithVariable("http", required: false, helpCallback: nameof(HttpHelp))]
        public Task<CommandResult> Http(CommandExecutionContext ctx)
        {
            var inputVar = ctx.ExecutingCommand.HasVariable ? ctx.ExecutingCommand.Variable : ctx.Scope.ReadInputAsString();
            if (!string.IsNullOrEmpty(inputVar))
            {
                if (Uri.TryCreate(inputVar, UriKind.Absolute, out var uri))
                {
                    try
                    {
                        var res = client.DownloadString(uri);
                        ctx.Scope.WriteString(res);
                        return CommandResult.SuccessfulTask;
                    }
                    catch (Exception ex)
                    {
                        ctx.Error.WithForegroundColor().Red().WriteLine(ex.Message);
                    }
                }
                else
                {
                    ctx.Error.WithForegroundColor().Red().Write($"Invalid URI: ").WriteLine(inputVar);
                }
            }
            else
            {
                ctx.Error.WithForegroundColor().Red().WriteLine($"No URI provided");
            }
            return CommandResult.ErrorTask;
        }

        public static void HttpHelp(Out o)
        {
            o.Write("Http utilities.");
        }

        //https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods
        private static string[] HttpMethods = new[]
        {
            "GET",
            "POST",
            "HEAD",
            "DELETE",
            "CONNECT",
            "OPTIONS",
            "PATCH",
            "PUT",
            "TRACE"
        };

        [VariableOption(1, "-m", "--method", false, nameof(MethodHelp), nameof(MethodSuggestions))]
        private static Task<CommandResult> Method(CommandExecutionContext ctx)
        {
            ctx.Scope.Add("--method", ctx.ExecutingCommand.Variable);
            return CommandResult.SuccessfulTask;
        }
     
        public static void MethodHelp(Out o)
        {
            o.Write("Http request method.");
        }

        public static IEnumerable<Suggestion> MethodSuggestions(string var)
        {
            return HttpMethods.Select(c => new Suggestion(c, c));
        }

    }

}
