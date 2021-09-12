using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Gint
{
    public class CommandRegistry
    {
        private readonly Dictionary<string, CommandEntry> registry = new Dictionary<string, CommandEntry>();
        public IReadOnlyDictionary<string, CommandEntry> Registry => registry;

        internal CommandRegistry()
        {
            var help = new HelpCommandDefinition();
            help.Register(this);
        }

        public class AddOptionsBuilder
        {
            private readonly List<Option> options = new List<Option>();
            private readonly string commandName;
            private readonly CommandRegistry registry;

            internal AddOptionsBuilder(string commandName, CommandRegistry registry)
            {
                this.commandName = commandName;
                this.registry = registry;
            }

            public AddOptionsBuilder AddOption(int priority, string argument, string longArgument, bool allowMultiple, HelpCallback helpCallback, ExecutionBlock callback)
            {
                var option = new Option(priority, argument, longArgument, allowMultiple, callback, helpCallback);
                options.Add(option);
                registry.Registry[commandName].Options = options.ToArray();
                return this;
            }
            public AddOptionsBuilder AddVariableOption(int priority, string argument, string longArgument, bool allowMultiple, HelpCallback helpCallback, ExecutionBlock callback)
            {
                var option = new VariableOption(priority, argument, longArgument, allowMultiple, callback, helpCallback);
                options.Add(option);
                registry.Registry[commandName].Options = options.ToArray();
                return this;
            }

        }
        public AddOptionsBuilder AddCommand(string commandName, HelpCallback helpCallback, ExecutionBlock callback)
        {
            Add(new Command(commandName, helpCallback, callback));
            return new AddOptionsBuilder(commandName, this);
        }


        public void Add(Command command, params Option[] options)
        {
            if (registry.ContainsKey(command.CommandName))
            {
                throw new CommandRegistrationException($"Command {command.CommandName} has already been registered.");
            }

            registry.Add(command.CommandName, new CommandEntry(command, options?.ToArray() ?? new Option[0]));
        }

        public void AddDefinition(ICommandDefinition definition)
        {
            definition.Register(this);
        }

        public void AddDefinitions(IEnumerable<ICommandDefinition> definitions)
        {
            foreach (var d in definitions)
            {
                d.Register(this);
            }
        }

        public void ScanAttributes(object obj)
        {
            ScanForAttributes(this, obj);
        }

        public static CommandRegistry Empty => new CommandRegistry();

        public static CommandRegistry FromDefinitions(IEnumerable<ICommandDefinition> definitions)
        {
            var registry = new CommandRegistry();
            foreach (var d in definitions)
            {
                d.Register(registry);
            }
            return registry;
        }

        public static CommandRegistry FromAttributes(IEnumerable<object> types)
        {
            var registry = new CommandRegistry();

            foreach (var definition in types)
            {
                ScanForAttributes(registry, definition);
            }
            return registry;
        }

        private static void ScanForAttributes(CommandRegistry registry, object definition)
        {
            var methods = definition.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Command command = null;
            List<Option> options = new List<Option>();

            foreach (var m in methods)
            {
                ExecutionBlock GetExecutionBlock()
                {
                    var parameters = m.GetParameters().ToList();
                    if (parameters.Count != 3)
                        throw new CommandDiscoveryException($"ICommandDefinition {definition.GetType().FullName} method {m.Name} has wrong number of parameters. See ExecutionBlock delegate");
                    if (parameters[0].ParameterType != typeof(ICommandInput))
                        throw new CommandDiscoveryException($"ICommandDefinition {definition.GetType().FullName} method {m.Name} first parameter should be of type ICommandInput. See ExecutionBlock delegate");
                    if (parameters[1].ParameterType != typeof(CommandExecutionContext))
                        throw new CommandDiscoveryException($"ICommandDefinition {definition.GetType().FullName} method {m.Name} second parameter should be of type CommandExecutionContext. See ExecutionBlock delegate");
                    if (parameters[2].ParameterType != typeof(Func<Task>))
                        throw new CommandDiscoveryException($"ICommandDefinition {definition.GetType().FullName} method {m.Name} third parameter should be of type Func<Task>. See ExecutionBlock delegate");
                    if (m.ReturnType != typeof(Task<ICommandOutput>))
                        throw new CommandDiscoveryException($"ICommandDefinition {definition.GetType().FullName} method {m.Name} return type should be of type Task<ICommandOutput>. See ExecutionBlock delegate.");

                    return new ExecutionBlock((i, c, n) => (Task<ICommandOutput>)m.Invoke(definition, new object[] { i, c, n }));
                }
                HelpCallback GetHelpCallback(string name)
                {
                    var callback = methods.Where(c => c.Name == name).FirstOrDefault();
                    if (callback == null)
                        throw new CommandDiscoveryException($"ICommandDefinition {definition.GetType().FullName} has no help method {name} defined");

                    var parameters = callback.GetParameters().ToList();
                    if (parameters.Count != 1)
                        throw new CommandDiscoveryException($"ICommandDefinition {definition.GetType().FullName} method {callback.Name} has wrong number of parameters. See HelpCallback delegate");
                    if (parameters[0].ParameterType != typeof(Out))
                        throw new CommandDiscoveryException($"ICommandDefinition {definition.GetType().FullName} method {callback.Name} parameter should of type Out. See HelpCallback delegate");
                    if (callback.ReturnType != typeof(void))
                        throw new CommandDiscoveryException($"ICommandDefinition {definition.GetType().FullName} method {callback.Name} return type should be of type void. See HelpCallback delegate.");

                    return new HelpCallback((i) => m.Invoke(definition, new object[] { i }));
                }

                var cmdAttr = m.GetCustomAttribute(typeof(CommandAttribute));
                if (cmdAttr is CommandAttribute attr)
                {
                    command = new Command(attr.CommandName, GetHelpCallback(attr.HelpCallbackMethodName), GetExecutionBlock());
                    continue;
                }
                var optAttr = m.GetCustomAttribute(typeof(OptionAttribute));
                if (optAttr is OptionAttribute oatrr)
                {
                    options.Add(new Option(oatrr.Priority, oatrr.Argument, oatrr.LongArgument, oatrr.AllowMultiple, GetExecutionBlock(), GetHelpCallback(oatrr.HelpCallbackMethodName)));
                    continue;
                }
                var voptAttr = m.GetCustomAttribute(typeof(VariableOptionAttribute));
                if (voptAttr is VariableOptionAttribute voattr)
                {
                    options.Add(new VariableOption(voattr.Priority, voattr.Argument, voattr.LongArgument, voattr.AllowMultiple, GetExecutionBlock(), GetHelpCallback(voattr.HelpCallbackMethodName)));
                    continue;
                }
            }
            if (command != null)
            {
                registry.Add(command, options.ToArray());
            }
            else
            {
                //throw new CommandDiscoveryException($"ICommandDefinition {definition.GetType().FullName} has no commands registered");
            }
        }

        public static CommandRegistry DiscoverDefinitions(Assembly assembly)
        {
            var type = typeof(ICommandDefinition);
            var definitions = assembly.GetTypes()
             .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract)
             .Select(t => Activator.CreateInstance(t))
             .Cast<ICommandDefinition>();

            var registry = new CommandRegistry();

            foreach (var d in definitions)
            {
                d.Register(registry);
            }

            return registry;
        }

        public static CommandRegistry DiscoverAttributeDefinitions(Assembly assembly)
        {
            var type = typeof(IScanForAttributes);
            var definitions = assembly.GetTypes()
             .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract)
             .Select(t => Activator.CreateInstance(t));

            var registry = new CommandRegistry();

            foreach (var d in definitions)
            {
                ScanForAttributes(registry, d);
            }

            return registry;
        }
    }

}
