using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Gint.Tests")]
[assembly: InternalsVisibleTo("Gint.Benchmarks")]

namespace Gint.Pipes
{
    class Bootstrap
    {

        [ModuleInitializer]
        public static void Init()
        { }
    }
}
