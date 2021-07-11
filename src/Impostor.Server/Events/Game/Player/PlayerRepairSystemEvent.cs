using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Player
{
    public class PlayerRepairSystemEvent : IPlayerRepairSystemEvent
    {
        public PlayerRepairSystemEvent(IGame game, IClientPlayer clientPlayer, IInnerPlayerControl playerControl, SystemTypes systemType, byte amount)
        {
            Game = game;
            ClientPlayer = clientPlayer;
            PlayerControl = playerControl;
            SystemType = systemType;
            Amount = amount;
        }

        public IGame Game { get; }

        public IClientPlayer ClientPlayer { get; }

        public IInnerPlayerControl PlayerControl { get; }

        public SystemTypes SystemType { get; }

        public byte Amount { get; }
    }
}
