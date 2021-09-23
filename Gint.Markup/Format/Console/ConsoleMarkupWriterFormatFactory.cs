using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gint.Markup.Format
{
    public class ConsoleMarkupWriterFormatFactory
    {
        private readonly Dictionary<string, Func<IConsoleMarkupFormat>> registry;
        private readonly NoopFormat noopFormat = new();

        public ConsoleMarkupWriterFormatFactory()
        {
            var formatType = typeof(IConsoleMarkupFormat);
            var formats = Assembly.Load("Gint.Markup")
            .GetTypes()
            .Where(type => type.Namespace == "Gint.Markup.Format" && formatType.IsAssignableFrom(type) && !type.IsAbstract)
            .Select(t => new
            {
                Tag = ((IConsoleMarkupFormat)Activator.CreateInstance(t)).Tag,
                Factory = new Func<IConsoleMarkupFormat>(() => (IConsoleMarkupFormat)Activator.CreateInstance(t))
            });

            this.registry = formats.ToDictionary(c => c.Tag, c => c.Factory);
        }

        public IConsoleMarkupFormat GetFormat(string tag)
        {
            if (registry.ContainsKey(tag))
            {
                return registry[tag].Invoke();
            }
            else
            {
                return noopFormat;
            }
        }
    }

}
