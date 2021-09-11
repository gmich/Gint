using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Gint.Tests")]

namespace Gint
{
    class Bootstrap
    {

        [ModuleInitializer]
        public static void Init()
        { }
    }
}
