using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Impostor.Server.Plugins
{
    public class AssemblyInformation : IAssemblyInformation
    {
        private Assembly? _assembly;

        public AssemblyInformation(AssemblyName assemblyName, string path, bool isPlugin, bool isDefaultAssembly)
        {
            AssemblyName = assemblyName;
            Path = path;
            IsPlugin = isPlugin;
            IsDefaultAssembly = isDefaultAssembly;
        }

        public string Path { get; }

        public bool IsPlugin { get; }

        public bool IsDefaultAssembly { get; }

        public AssemblyName AssemblyName { get; }

        public Assembly Load(AssemblyLoadContext context)
        {
            if (_assembly != null)
            {
                return _assembly;
            }

            using var stream = File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.Read);

            _assembly = context.LoadFromStream(stream);

            return _assembly;
        }
    }
}
