using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Gint.UnitTests")]
[assembly: InternalsVisibleTo("Gint.Benchmarks")]
[assembly: InternalsVisibleTo("Gint.Terminal")]

namespace Gint
{
    class Bootstrap
    {

        [ModuleInitializer]
        public static void Init()
        { }
    }
}
