using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Gint.Tests")]
[assembly: InternalsVisibleTo("Gint.Benchmarks")]

namespace Gint.Markup
{
    class Bootstrap
    {

        [ModuleInitializer]
        public static void Init()
        { }
    }
}
