using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Plugins;
using Impostor.Server.Config;
using Impostor.Server.Plugins.Proxies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Plugins
{
    public class PluginManager : IPluginManager, IHostedService
    {
        private readonly ILogger<PluginManager> _logger;
        private readonly IOptions<PluginLoaderConfig> _pluginConfig;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly ServiceProxyCollection _serviceProxyCollection;

        private PluginAssemblyLoadContext? _currentContext;

        public PluginManager(ILogger<PluginManager> logger, IOptions<PluginLoaderConfig> pluginConfig, IConfiguration configuration, IServiceProvider serviceProvider, ServiceProxyCollection serviceProxyCollection)
        {
            _logger = logger;
            _pluginConfig = pluginConfig;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _serviceProxyCollection = serviceProxyCollection;
        }

        public IHost? CurrentHost { get; private set; }

        public List<PluginInformation> Plugins { get; } = new List<PluginInformation>();

        IReadOnlyList<IPluginInformation> IPluginManager.Plugins => Plugins.AsReadOnly();

        public Task StartAsync(CancellationToken cancellationToken)
        {
            CurrentHost = CreatePluginHost();
            return CurrentHost.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (CurrentHost != null)
            {
                await CurrentHost.StopAsync(cancellationToken);

                if (CurrentHost is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else
                {
                    CurrentHost.Dispose();
                }
            }
        }

        private IDictionary<string, IAssemblyInformation> GetAssemblyInformation()
        {
            // Add the plugins and libraries.
            var config = _pluginConfig.Value;
            var pluginPaths = new List<string>(config.Paths);
            var libraryPaths = new List<string>(config.LibraryPaths);
            CheckPaths(pluginPaths);
            CheckPaths(libraryPaths);

            var rootFolder = Directory.GetCurrentDirectory();

            pluginPaths.Add(Path.Combine(rootFolder, "plugins"));
            libraryPaths.Add(Path.Combine(rootFolder, "libraries"));

            var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
            matcher.AddInclude("*.dll");
            matcher.AddExclude("Impostor.Api.dll");
            matcher.AddExclude("Impostor.Server.dll");

            var assembliesByName = AssemblyLoadContext.Default.Assemblies
                .Where(a => a.GetName().Name != null)
                .ToDictionary(
                    a => a.GetName().Name!,
                    a => (IAssemblyInformation)new LoadedAssemblyInformation(a, true));

            RegisterAssemblies(assembliesByName, pluginPaths, matcher, true);
            RegisterAssemblies(assembliesByName, libraryPaths, matcher, false);

            return assembliesByName;
        }

        private IHost CreatePluginHost()
        {
            var assembliesByName = GetAssemblyInformation();
            _currentContext ??= new PluginAssemblyLoadContext(assembliesByName);

            var pluginHostBuilder = new HostBuilder()
                .UseConsoleLifetime(options => options.SuppressStatusMessages = true)
                .UseContentRoot(_configuration.GetValue<string>(HostDefaults.ContentRootKey))
                .ConfigureHostConfiguration(builder => { builder.AddConfiguration(_configuration); })
                .UseServiceProviderFactory(_ => new DefaultServiceProviderFactory());

            Plugins.Clear();
            Plugins.AddRange(GetPluginInformation(assembliesByName, _currentContext));

            foreach (var plugin in Plugins)
            {
                plugin.Startup?.ConfigureHost(pluginHostBuilder);
            }

            pluginHostBuilder.ConfigureServices(services =>
            {
                services.AddHostedService<PluginLoaderService>();

                foreach (var (type, lifetime) in _serviceProxyCollection.ServiceTypes)
                {
                    services.Add(new ServiceDescriptor(
                        type,
                        _ => _serviceProvider.GetService(type)!,
                        lifetime));
                }

                foreach (var plugin in Plugins)
                {
                    plugin.Startup?.ConfigureServices(services);
                }
            });

            return pluginHostBuilder.Build();
        }

        private List<PluginInformation> GetPluginInformation(IDictionary<string, IAssemblyInformation> assembliesByName, PluginAssemblyLoadContext context)
        {
            var plugins = new List<PluginInformation>();
            var pluginAssemblies = assembliesByName
                .Values
                .Where(a => a.IsPlugin)
                .Select(a => context.LoadFromAssemblyName(a.AssemblyName))
                .ToList();

            foreach (var assembly in pluginAssemblies)
            {
                // Find plugin startup.
                var pluginStartup = assembly
                    .GetTypes()
                    .Where(t => typeof(IPluginStartup).IsAssignableFrom(t) && t.IsClass)
                    .ToList();

                if (pluginStartup.Count > 1)
                {
                    _logger.LogWarning("A plugin may only define zero or one IPluginStartup implementation ({0}).", assembly);
                    continue;
                }

                // Find plugin.
                var plugin = assembly
                    .GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t)
                                && t.IsClass
                                && !t.IsAbstract
                                && t.GetCustomAttribute<ImpostorPluginAttribute>() != null)
                    .ToList();

                if (plugin.Count != 1)
                {
                    _logger.LogWarning("A plugin must define exactly one IPlugin or PluginBase implementation ({0}).", assembly);
                    continue;
                }

                // Save plugin.
                plugins.Add(new PluginInformation(
                    pluginStartup
                        .Select(Activator.CreateInstance)
                        .Cast<IPluginStartup>()
                        .FirstOrDefault(),
                    plugin.Single()));
            }

            return OrderPlugins(plugins);
        }

        private void CheckPaths(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                if (!Directory.Exists(path))
                {
                    _logger.LogWarning("Path {path} was specified in the PluginLoader configuration, but this folder doesn't exist!", path);
                }
            }
        }

        private void RegisterAssemblies(
            IDictionary<string, IAssemblyInformation> assemblies,
            IEnumerable<string> paths,
            Matcher matcher,
            bool isPlugin)
        {
            foreach (var path in paths.SelectMany(matcher.GetResultsInFullPath))
            {
                AssemblyName name;

                try
                {
                    name = AssemblyName.GetAssemblyName(path);
                }
                catch (BadImageFormatException)
                {
                    continue;
                }

                if (name.Name == null)
                {
                    _logger.LogWarning(
                        "Skipped {0} because the assembly name could not be resolved",
                        path);
                    continue;
                }

                if (assemblies.TryGetValue(name.Name, out var loaded))
                {
                    var version = name.Version;
                    var loadedVersion = loaded.AssemblyName.Version;

                    if (loadedVersion != null && version != null && version.Major != loadedVersion.Major)
                    {
                        _logger.LogError(
                            "Assembly {0} is already loaded with version {1} while loading version {2}",
                            name.Name,
                            loadedVersion,
                            version);
                    }
                    else
                    {
                        _logger.LogDebug("Skipped assembly {0} because it's already loaded", name.Name);
                    }

                    continue;
                }

                _logger.LogTrace("Registered assembly {name} v{version} ({path})", name.Name, name.Version, path);

                assemblies.Add(name.Name, new AssemblyInformation(name, path, isPlugin, false));
            }
        }

        private List<PluginInformation> OrderPlugins(IEnumerable<PluginInformation> plugins)
        {
            var pluginDictionary = new Dictionary<string, PluginInformation>();
            var hardDependencies = new Dictionary<string, List<string>>();

            foreach (var plugin in plugins)
            {
                pluginDictionary[plugin.Id] = plugin;
                hardDependencies[plugin.Id] = plugin
                    .Dependencies
                    .Where(p => p.DependencyType == DependencyType.HardDependency)
                    .Select(p => p.Id)
                    .ToList();
            }

            var presentPlugins = pluginDictionary.Keys.ToList();

            // Check whether the Hard Dependencies are present and remove those without.
            var checkedPlugins = CheckHardDependencies(presentPlugins, hardDependencies);

            var dependencyGraph = checkedPlugins.ToDictionary(p => p, _ => new List<string>());

            foreach (var plugin in checkedPlugins)
            {
                foreach (var dependency in pluginDictionary[plugin].Dependencies.Where(d => checkedPlugins.Contains(d.Id)))
                {
                    if (dependency.DependencyType == DependencyType.LoadBefore)
                    {
                        dependencyGraph[dependency.Id].Add(plugin);
                    }
                    else
                    {
                        dependencyGraph[plugin].Add(dependency.Id);
                    }
                }
            }

            var processed = new List<string>();
            var ordered = new List<PluginInformation>();
            foreach (var plugin in checkedPlugins)
            {
                if (!processed.Contains(plugin))
                {
                    RecursiveOrder(plugin, dependencyGraph, processed, ordered, pluginDictionary);
                }
            }

            return ordered;
        }

        private List<string> CheckHardDependencies(
            List<string> plugins,
            IReadOnlyDictionary<string, List<string>> hardDependencies)
        {
            foreach (var plugin in plugins)
            {
                if (!hardDependencies.ContainsKey(plugin))
                {
                    continue;
                }

                foreach (var dependency in hardDependencies[plugin].Where(dependency => !plugins.Contains(dependency)))
                {
                    _logger.LogError(
                        "The plugin {plugin} has defined the plugin {dependency} as a hard dependency but its not present! {plugin} will not loaded.",
                        plugin,
                        dependency,
                        plugin
                    );

                    // Remove the plugin from the plugins to load.
                    plugins.Remove(plugin);

                    // Since other plugins might have defined the removed plugin as a hard dependency a recheck is necessary.
                    return CheckHardDependencies(plugins, hardDependencies);
                }
            }

            return plugins;
        }

        private void RecursiveOrder(
            string plugin,
            IReadOnlyDictionary<string, List<string>> dependencyGraph,
            ICollection<string> processed,
            ICollection<PluginInformation> ordered,
            IReadOnlyDictionary<string, PluginInformation> pluginDictionary)
        {
            processed.Add(plugin);

            foreach (var dependency in dependencyGraph[plugin])
            {
                // First add the dependencies using a recursive call before adding itself.
                if (!processed.Contains(dependency))
                {
                    RecursiveOrder(dependency, dependencyGraph, processed, ordered, pluginDictionary);
                }
            }

            ordered.Add(pluginDictionary[plugin]);
        }
    }
}
