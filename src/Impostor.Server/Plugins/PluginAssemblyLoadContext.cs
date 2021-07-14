using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;

namespace Impostor.Server.Plugins
{
    public class PluginAssemblyLoadContext : AssemblyLoadContext, IDisposable
    {
        private readonly IDictionary<string, IAssemblyInformation> _assembliesByName;

        public PluginAssemblyLoadContext(IDictionary<string, IAssemblyInformation> assembliesByName)
            : base(true)
        {
            _assembliesByName = assembliesByName;
        }

        public void Dispose()
        {
            _assembliesByName.Clear();
            this.Unload();
        }

        protected override Assembly? Load(AssemblyName name)
        {
            return name.Name != null && _assembliesByName.TryGetValue(name.Name, out var assemblyInfo)
                ? assemblyInfo.Load(this)
                : null;
        }
    }
}
