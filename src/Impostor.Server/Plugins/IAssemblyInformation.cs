using System.Reflection;
using System.Runtime.Loader;

namespace Impostor.Server.Plugins
{
    public interface IAssemblyInformation
    {
        AssemblyName AssemblyName { get; }

        bool IsPlugin { get; }

        bool IsDefaultAssembly { get; }

        Assembly Load(AssemblyLoadContext context);
    }
}
