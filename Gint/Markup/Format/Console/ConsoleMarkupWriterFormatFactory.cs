using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gint.Markup.Format
{
    public class ConsoleMarkupWriterFormatFactory
    {
        private readonly Dictionary<string, Func<IMarkupFormat>> registry;
        private readonly NoopFormat noopFormat = new();

        public ConsoleMarkupWriterFormatFactory()
        {
            var formatType = typeof(IMarkupFormat);
            var formats = Assembly.Load("Gint")
            .GetTypes()
            .Where(type => type.Namespace == "Gint.Markup.Format" && formatType.IsAssignableFrom(type) && !type.IsAbstract)
            .Select(t => new
            {
                Tag = ((IMarkupFormat)Activator.CreateInstance(t)).Tag,
                Factory = new Func<IMarkupFormat>(() => (IMarkupFormat)Activator.CreateInstance(t))
            });

            this.registry = formats.ToDictionary(c => c.Tag, c => c.Factory);
        }

        public IMarkupFormat GetFormat(string tag)
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
