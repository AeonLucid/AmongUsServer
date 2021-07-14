using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Maps;
using Impostor.Api.Net;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects.ShipStatus;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Server.Net.Inner.Objects.Systems;
using Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.ShipStatus
{
    internal abstract class InnerShipStatus : InnerNetObject, IInnerShipStatus
    {
        private readonly Dictionary<SystemTypes, ISystemType> _systems = new Dictionary<SystemTypes, ISystemType>();

        protected InnerShipStatus(ICustomMessageManager<ICustomRpc> customMessageManager, Game game) : base(customMessageManager, game)
        {
            Components.Add(this);
        }

        public abstract IMapData Data { get; }

        public abstract Dictionary<int, bool> Doors { get; }

        public abstract float SpawnRadius { get; }

        public abstract Vector2 InitialSpawnCenter { get; }

        public abstract Vector2 MeetingSpawnCenter { get; }

        internal override ValueTask OnSpawnAsync()
        {
            for (var i = 0; i < Doors.Count; i++)
            {
                Doors.Add(i, false);
            }

            AddSystems(_systems);
            _systems.Add(SystemTypes.Sabotage, new SabotageSystemType(_systems.Values.OfType<IActivatable>().ToArray()));

            return base.OnSpawnAsync();
        }

        public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override async ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            if (!await ValidateHost(CheatContext.Deserialize, sender) || !await ValidateBroadcast(CheatContext.Deserialize, sender, target))
            {
                return;
            }

            while (reader.Position < reader.Length)
            {
                var messageReader = reader.ReadMessage();
                var type = (SystemTypes)messageReader.Tag;
                if (_systems.TryGetValue(type, out var value))
                {
                    value.Deserialize(messageReader, initialState);
                }
            }
        }

        public override async ValueTask<bool> HandleRpcAsync(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            if (!await ValidateCmd(call, sender, target))
            {
                return false;
            }

            switch (call)
            {
                case RpcCalls.CloseDoorsOfType:
                {
                    if (!await ValidateImpostor(call, sender, sender.Character!.PlayerInfo))
                    {
                        return false;
                    }

                    Rpc27CloseDoorsOfType.Deserialize(reader, out var systemType);
                    break;
                }

                case RpcCalls.RepairSystem:
                {
                    Rpc28RepairSystem.Deserialize(reader, Game, out var systemType, out var player, out var amount);

                    if (systemType == SystemTypes.Sabotage && !await ValidateImpostor(call, sender, sender.Character!.PlayerInfo))
                    {
                        return false;
                    }

                    break;
                }

                case RpcCalls.UpdateSystem:
                {
                    Rpc35UpdateSystem.Deserialize(reader, Game, out var systemType, out var playerControl, out var sId, out var state, out var ventId);
                    break;
                }

                default:
                    return await base.HandleRpcAsync(sender, target, call, reader);
            }

            return true;
        }

        public virtual Vector2 GetSpawnLocation(InnerPlayerControl player, int numPlayers, bool initialSpawn)
        {
            var vector = new Vector2(0, 1);
            vector = Rotate(vector, (player.PlayerId - 1) * (360f / numPlayers));
            vector *= this.SpawnRadius;
            return (initialSpawn ? this.InitialSpawnCenter : this.MeetingSpawnCenter) + vector + new Vector2(0f, 0.3636f);
        }

        protected virtual void AddSystems(Dictionary<SystemTypes, ISystemType> systems)
        {
            systems.Add(SystemTypes.Electrical, new SwitchSystem());
            systems.Add(SystemTypes.MedBay, new MedScanSystem());
        }

        private static Vector2 Rotate(Vector2 self, float degrees)
        {
            var f = 0.017453292f * degrees;
            var cos = MathF.Cos(f);
            var sin = MathF.Sin(f);

            return new Vector2((self.X * cos) - (sin * self.Y), (self.X * sin) + (cos * self.Y));
        }
    }
}
