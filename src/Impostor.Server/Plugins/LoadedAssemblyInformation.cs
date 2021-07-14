using System.Reflection;
using System.Runtime.Loader;

namespace Impostor.Server.Plugins
{
    public class LoadedAssemblyInformation : IAssemblyInformation
    {
        private readonly Assembly _assembly;

        public LoadedAssemblyInformation(Assembly assembly, bool isDefaultAssembly)
        {
            AssemblyName = assembly.GetName();
            _assembly = assembly;
            IsDefaultAssembly = isDefaultAssembly;
        }

        public AssemblyName AssemblyName { get; }

        public bool IsPlugin => false;

        public bool IsDefaultAssembly { get; }

        public Assembly Load(AssemblyLoadContext context)
        {
            return _assembly;
        }
    }
}
