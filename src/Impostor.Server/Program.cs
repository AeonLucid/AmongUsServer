using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Manager;
using Impostor.Api.Net.Messages;
using Impostor.Api.Plugins;
using Impostor.Api.Utils;
using Impostor.Hazel.Extensions;
using Impostor.Server.Config;
using Impostor.Server.Events;
using Impostor.Server.Net;
using Impostor.Server.Net.Custom;
using Impostor.Server.Net.Factories;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Messages;
using Impostor.Server.Net.Redirector;
using Impostor.Server.Plugins;
using Impostor.Server.Plugins.Proxies;
using Impostor.Server.Recorder;
using Impostor.Server.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Serilog;
using Serilog.Events;

namespace Impostor.Server
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Starting Impostor v{0}", DotnetUtils.Version);
                await CreateHostBuilder(args).RunConsoleAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Impostor terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
#if DEBUG
                .UseEnvironment(Environment.GetEnvironmentVariable("IMPOSTOR_ENV") ?? "Development")
#else
                .UseEnvironment("Production")
#endif
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("config.json", true);
                    builder.AddJsonFile("config.Development.json", true);
                    builder.AddEnvironmentVariables(prefix: "IMPOSTOR_");
                    builder.AddCommandLine(args);
                })
                .ConfigureServices((host, services) =>
                {
                    var debug = host.Configuration
                        .GetSection(DebugConfig.Section)
                        .Get<DebugConfig>() ?? new DebugConfig();

                    var redirector = host.Configuration
                        .GetSection(ServerRedirectorConfig.Section)
                        .Get<ServerRedirectorConfig>() ?? new ServerRedirectorConfig();

                    var announcementsServer = host.Configuration
                        .GetSection(AnnouncementsServerConfig.Section)
                        .Get<AnnouncementsServerConfig>() ?? new AnnouncementsServerConfig();

                    var authServer = host.Configuration
                        .GetSection(AuthServerConfig.Section)
                        .Get<AuthServerConfig>() ?? new AuthServerConfig();

                    services.AddSingleton<ServerEnvironment>();
                    services.AddSingleton<IServerEnvironment>(p => p.GetRequiredService<ServerEnvironment>());
                    services.AddSingleton<IDateTimeProvider, RealDateTimeProvider>();

                    services.Configure<DebugConfig>(host.Configuration.GetSection(DebugConfig.Section));
                    services.Configure<AntiCheatConfig>(host.Configuration.GetSection(AntiCheatConfig.Section));
                    services.Configure<ServerConfig>(host.Configuration.GetSection(ServerConfig.Section));
                    services.Configure<AnnouncementsServerConfig>(host.Configuration.GetSection(AnnouncementsServerConfig.Section));
                    services.Configure<AuthServerConfig>(host.Configuration.GetSection(AuthServerConfig.Section));
                    services.Configure<ServerRedirectorConfig>(host.Configuration.GetSection(ServerRedirectorConfig.Section));
                    services.Configure<PluginLoaderConfig>(host.Configuration.GetSection(PluginLoaderConfig.Section));

                    if (redirector.Enabled)
                    {
                        if (!string.IsNullOrEmpty(redirector.Locator?.Redis))
                        {
                            // When joining a game, it retrieves the game server ip from redis.
                            // When a game has been created on this node, it stores the game code with its ip in redis.
                            services.AddSingleton<INodeLocator, NodeLocatorRedis>();

                            // Dependency for the NodeLocatorRedis.
                            services.AddStackExchangeRedisCache(options =>
                            {
                                options.Configuration = redirector.Locator.Redis;
                                options.InstanceName = "ImpostorRedis";
                            });
                        }
                        else if (!string.IsNullOrEmpty(redirector.Locator?.UdpMasterEndpoint))
                        {
                            services.AddSingleton<INodeLocator, NodeLocatorUdp>();

                            if (redirector.Master)
                            {
                                services.AddHostedService<NodeLocatorUdpService>();
                            }
                        }
                        else
                        {
                            throw new Exception("Missing a valid NodeLocator config.");
                        }

                        // Use the configuration as source for the list of nodes to provide
                        // when creating a game.
                        services.AddSingleton<INodeProvider, NodeProviderConfig>();
                    }
                    else
                    {
                        // Redirector is not enabled but the dependency is still required.
                        // So we provide one that ignores all calls.
                        services.AddSingleton<INodeLocator, NodeLocatorNoOp>();
                    }

                    services.AddSingleton<ClientManager>();
                    services.AddSingleton<IClientManager>(p => p.GetRequiredService<ClientManager>());

                    if (redirector.Enabled && redirector.Master)
                    {
                        services.AddSingleton<IClientFactory, ClientFactory<ClientRedirector>>();

                        // For a master server, we don't need a GameManager.
                    }
                    else
                    {
                        if (debug.GameRecorderEnabled)
                        {
                            services.AddSingleton<ObjectPoolProvider>(new DefaultObjectPoolProvider());
                            services.AddSingleton<ObjectPool<PacketSerializationContext>>(serviceProvider =>
                            {
                                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                                var policy = new PacketSerializationContextPooledObjectPolicy();
                                return provider.Create(policy);
                            });

                            services.AddSingleton<PacketRecorder>();
                            services.AddHostedService(sp => sp.GetRequiredService<PacketRecorder>());
                            services.AddSingleton<IClientFactory, ClientFactory<ClientRecorder>>();
                        }
                        else
                        {
                            services.AddSingleton<IClientFactory, ClientFactory<Client>>();
                        }

                        services.AddSingleton<GameManager>();
                        services.AddSingleton<IGameManager>(p => p.GetRequiredService<GameManager>());
                    }

                    services.AddEventPools();
                    services.AddHazel();
                    services.AddSingleton<ICustomMessageManager<ICustomRootMessage>, CustomMessageManager<ICustomRootMessage>>();
                    services.AddSingleton<ICustomMessageManager<ICustomRpc>, CustomMessageManager<ICustomRpc>>();
                    services.AddSingleton<IMessageWriterProvider, MessageWriterProvider>();
                    services.AddSingleton<IGameCodeFactory, GameCodeFactory>();
                    services.AddSingleton<IEventManager, EventManager>();
                    services.AddSingleton<Matchmaker>();
                    services.AddHostedService<MatchmakerService>();

                    if (announcementsServer.Enabled)
                    {
                        services.AddHostedService<AnnouncementsService>();
                    }

                    if (authServer.Enabled)
                    {
                        services.AddHostedService<AuthService>();
                    }

                    services.AddSingleton<PluginManager>();
                    services.AddSingleton<IPluginManager>(p => p.GetRequiredService<PluginManager>());
                    services.AddHostedService(p => p.GetRequiredService<PluginManager>());
                })
                .UseSerilog((context, loggerConfiguration) =>
                {
#if DEBUG
                    var logLevel = LogEventLevel.Debug;
#else
                    var logLevel = LogEventLevel.Information;
#endif

                    if (args.Contains("--verbose"))
                    {
                        logLevel = LogEventLevel.Verbose;
                    }
                    else if (args.Contains("--errors-only"))
                    {
                        logLevel = LogEventLevel.Error;
                    }

                    loggerConfiguration
                        .MinimumLevel.Is(logLevel)
#if DEBUG
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
#else
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
#endif
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .ReadFrom.Configuration(context.Configuration);
                })
                .ConfigureServices(services =>
                {
                    services.AddTransient(serviceProvider => serviceProvider.GetRequiredService<PluginManager>().CurrentHost?.Services.GetServices<IEventListener>() ?? Enumerable.Empty<IEventListener>());

                    var apiAssembly = typeof(IPlugin).Assembly;
                    var serilogAssembly = typeof(Serilog.ILogger).Assembly;

                    services.AddSingleton(new ServiceProxyCollection(
                        services
                            .Where(t => t.ServiceType.Assembly == apiAssembly || t.ServiceType.Assembly == serilogAssembly || t.ServiceType == typeof(PluginManager) || t.ServiceType == typeof(ILoggerFactory))
                            .Select(t => new ServiceRegistration(t.ServiceType, t.Lifetime))
                            .Distinct()
                            .ToArray()));
                });
        }
    }
}
