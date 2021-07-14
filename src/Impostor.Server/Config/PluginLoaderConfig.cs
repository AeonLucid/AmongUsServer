using System.Collections.Generic;

namespace Impostor.Server.Config
{
    public class PluginLoaderConfig
    {
        public const string Section = "PluginLoader";

        public List<string> Paths { get; set; } = new List<string>();

        public List<string> LibraryPaths { get; set; } = new List<string>();
    }
}
