using System;
using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Net;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Server.Events.Player;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects.Components
{
    internal partial class InnerPlayerPhysics : InnerNetObject
    {
        private readonly ILogger<InnerPlayerPhysics> _logger;
        private readonly InnerPlayerControl _playerControl;
        private readonly IEventManager _eventManager;

        public InnerPlayerPhysics(ICustomMessageManager<ICustomRpc> customMessageManager, Game game, ILogger<InnerPlayerPhysics> logger, InnerPlayerControl playerControl, IEventManager eventManager) : base(customMessageManager, game)
        {
            _logger = logger;
            _playerControl = playerControl;
            _eventManager = eventManager;
        }

        public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override async ValueTask<bool> HandleRpcAsync(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            if (!await ValidateOwnership(call, sender))
            {
                return false;
            }

            _logger.LogDebug("Rpc call {id}", call);

            switch (call)
            {
                case RpcCalls.EnterVent:
                case RpcCalls.ExitVent:
                {
                    if (!await ValidateImpostor(call, sender, _playerControl.PlayerInfo))
                    {
                        return false;
                    }

                    int ventId;

                    switch (call)
                    {
                        case RpcCalls.EnterVent:
                            Rpc19EnterVent.Deserialize(reader, out ventId);
                            break;
                        case RpcCalls.ExitVent:
                            Rpc20ExitVent.Deserialize(reader, out ventId);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(call), call, null);
                    }

                    if (!Game.GameNet.ShipStatus!.Data.Vents.TryGetValue(ventId, out var vent))
                    {
                        if (await sender.Client.ReportCheatAsync(call, "Client interacted with nonexistent vent"))
                        {
                            return false;
                        }

                        break;
                    }

                    switch (call)
                    {
                        case RpcCalls.EnterVent:
                            await _eventManager.CallAsync(new PlayerEnterVentEvent(Game, sender, _playerControl, vent));
                            break;
                        case RpcCalls.ExitVent:
                            await _eventManager.CallAsync(new PlayerExitVentEvent(Game, sender, _playerControl, vent));
                            break;
                    }

                    break;
                }

                case RpcCalls.BootFromVent:
                {
                    Rpc34BootFromVent.Deserialize(reader, out var ventId);
                    break;
                }

                case RpcCalls.ClimbLadder:
                    Rpc31ClimbLadder.Deserialize(reader, out var ladderId, out var lastClimbLadderSid);
                    break;

                default:
                    return await base.HandleRpcAsync(sender, target, call, reader);
            }

            return true;
        }
    }
}
