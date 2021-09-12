using System;
using System.Linq;

namespace Gint
{
    using static ExecutionBlockUtilities;

    internal class CommandBinder
    {
        private readonly ExpressionSyntax root;
        private readonly DiagnosticCollection diagnostics = new DiagnosticCollection();
        private readonly CommandRegistry commandRegistry;

        public CommandBinder(ExpressionSyntax root, CommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
            this.root = root;
        }

        public DiagnosticCollection Diagnostics => diagnostics;

        public BoundNode Bind()
        {
            return BindExpression(root);
        }

        private BoundNode BindExpression(ExpressionSyntax node)
        {
            switch (node.Kind)
            {
                case CommandTokenKind.CommandExpression:
                    return BindCommandExpression((CommandExpressionSyntax)node);
                case CommandTokenKind.PipedCommandExpression:
                    return BindPipedCommandExpression((PipedCommandExpressionSyntax)node);
                default:
                    throw new Exception("");
            }
        }

        private BoundPipeline BindPipedCommandExpression(PipedCommandExpressionSyntax n)
        {
            var boundFirstCommand = BindExpression(n.FirstCommand);
            var boundPipe = BindPipeExpression(n.PipeToken);
            var boundSecondCommand = BindExpression(n.SecondCommand);

            return new BoundPipeline(boundFirstCommand, boundPipe, boundSecondCommand);
        }

        private BoundPipe BindPipeExpression(PipeExpressionSyntax n)
        {
            return new BoundPipe(n);
        }

        private CommandEntry GetCommand(CommandExpressionSyntax n)
        {
            var cmd = n.CommandToken.Value;
            if (!commandRegistry.Registry.ContainsKey(cmd))
            {
                diagnostics.ReportCommandUnknown(n.CommandToken.Span, cmd);
                return new CommandEntry(new Command(cmd, NoopHelp, NoopExecutionBlock), new Option[0]);
            }
            else
            {
                return commandRegistry.Registry[cmd];
            }
        }
        private BoundCommand BindCommandExpression(CommandExpressionSyntax n)
        {
            var cmd = GetCommand(n);

            var boundOptions = n.Options
                .Select(c =>
                    BindOption(c, cmd)
                )
                .OrderBy(c => c.Priority)
                .ToArray();

            return new BoundCommand(cmd.Command, n.CommandToken, boundOptions);
        }

        private BoundOptionExpression BindOption(ExpressionSyntax node, CommandEntry command)
        {
            switch (node.Kind)
            {
                case CommandTokenKind.OptionExpression:
                    return BindOptionExpression((OptionExpressionSyntax)node, command);
                case CommandTokenKind.VariableOptionExpression:
                    return BindVariableOptionExpression((VariableOptionExpressionSyntax)node, command);
                default:
                    throw new Exception("");
            }
        }
        private BoundOptionExpression BindVariableOptionExpression(VariableOptionExpressionSyntax n, CommandEntry command)
        {
            var optionName = n.OptionToken.Value;
            var option = command.Options.Where(c => c.Argument == optionName || c.LongArgument == optionName).FirstOrDefault();

            if (option == null)
            {
                diagnostics.ReportOptionUnknown(n.OptionToken.Span, optionName);
                return new BoundVariableOption(string.Empty, 0, new VariableOption(0, optionName, optionName, NoopExecutionBlock, NoopHelp), string.Empty, n);
            }
            else if (option is VariableOption vop)
            {
                return new BoundVariableOption(vop.Argument, option.Priority, vop, n.VariableToken.Value, n);
            }
            else
            {
                diagnostics.ReportUnecessaryVariable(n.OptionToken.Span);
                return new BoundVariableOption(string.Empty, 0, new VariableOption(0, optionName, optionName, NoopExecutionBlock, NoopHelp), string.Empty, n);
            }
        }

        private BoundOptionExpression BindOptionExpression(OptionExpressionSyntax n, CommandEntry command)
        {
            var optionName = n.OptionToken.Value;
            var option = command.Options.Where(c => c.Argument == optionName || c.LongArgument == optionName).FirstOrDefault();

            if (option == null)
            {
                diagnostics.ReportOptionUnknown(n.OptionToken.Span, optionName);
                return new BoundOption(string.Empty, 0, new Option(0, optionName, optionName, NoopExecutionBlock, NoopHelp), n);
            }
            else if (option is VariableOption vop)
            {
                diagnostics.ReportMissingVariable(n.Span);
                return new BoundOption(string.Empty, 0, new Option(0, optionName, optionName, NoopExecutionBlock, NoopHelp), n);
            }
            else
            {
                return new BoundOption(option.Argument, option.Priority, option, n);
            }
        }
    }
}
