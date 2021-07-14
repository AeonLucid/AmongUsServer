using System;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Plugins
{
    public class PluginLoaderService : IHostedService
    {
        private readonly ILogger<PluginLoaderService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly PluginManager _pluginManager;

        public PluginLoaderService(ILogger<PluginLoaderService> logger, IServiceProvider serviceProvider, PluginManager pluginManager)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _pluginManager = pluginManager;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Enabling plugins.");

            foreach (var plugin in _pluginManager.Plugins)
            {
                _logger.LogInformation("Enabling plugin {0}.", plugin);

                // Create instance and inject services.
                plugin.Instance = (IPlugin)ActivatorUtilities.CreateInstance(_serviceProvider, plugin.PluginType);

                // Enable plugin.
                await plugin.Instance.EnableAsync();
            }

            _logger.LogInformation(
                _pluginManager.Plugins.Count == 1
                    ? "Enabled {0} plugin."
                    : "Enabled {0} plugins.",
                _pluginManager.Plugins.Count
            );
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Disable all plugins with a valid instance set.
            // In the case of a failed startup, some can be null.
            foreach (var plugin in _pluginManager.Plugins)
            {
                if (plugin.Instance != null)
                {
                    _logger.LogInformation("Disabling plugin {0}.", plugin);

                    // Disable plugin.
                    await plugin.Instance.DisableAsync();
                    plugin.Instance = null;
                }
            }
        }
    }
}
