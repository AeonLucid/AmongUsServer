using System.Linq;
using System.Threading.Tasks;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Plugins;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.Example
{
    [ImpostorPlugin("gg.impostor.example")]
    public class ExamplePlugin : PluginBase
    {
        private readonly ILogger<ExamplePlugin> _logger;
        private readonly IGameManager _gameManager;
        private readonly IPluginManager _pluginManager;

        public ExamplePlugin(ILogger<ExamplePlugin> logger, IGameManager gameManager, IPluginManager pluginManager)
        {
            _logger = logger;
            _gameManager = gameManager;
            _pluginManager = pluginManager;
        }

        public override async ValueTask EnableAsync()
        {
            _logger.LogInformation("Example is being enabled.");

            _logger.LogInformation("Following plugins are installed: {@plugins}", _pluginManager.Plugins.Select(x => $"{x.Id} ({x.Version})"));

            var game = await _gameManager.CreateAsync(new GameOptionsData());
            if (game == null)
            {
                _logger.LogWarning("Example game creation was cancelled");
            }
            else
            {
                game.DisplayName = "Example game";
                await game.SetPrivacyAsync(true);

                _logger.LogInformation("Created game {0}.", game.Code.Code);
            }
        }

        public override ValueTask DisableAsync()
        {
            _logger.LogInformation("Example is being disabled.");
            return default;
        }
    }
}
