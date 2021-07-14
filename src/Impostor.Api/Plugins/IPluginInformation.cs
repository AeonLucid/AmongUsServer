using System;

namespace Impostor.Api.Plugins
{
    public interface IPluginInformation
    {
        /// <summary>
        /// Gets the unique id of this plugin.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the human readable name of this plugin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the author of this plugin.
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Gets the version of this plugin.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets the optional startup object of this plugin.
        /// </summary>
        IPluginStartup? Startup { get; }

        /// <summary>
        /// Gets the instance of this plugin.
        /// </summary>
        IPlugin? Instance { get; }

        /// <summary>
        /// Gets the instance type of this plugin.
        /// </summary>
        Type PluginType { get; }
    }
}
