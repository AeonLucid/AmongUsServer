using System.Collections.Generic;
using System.Numerics;
using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Maps;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner.Objects.ShipStatus;
using Impostor.Server.Net.Inner.Objects.Systems;
using Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.ShipStatus
{
    internal class InnerPolusShipStatus : InnerShipStatus, IInnerPolusShipStatus
    {
        public InnerPolusShipStatus(ICustomMessageManager<ICustomRpc> customMessageManager, Game game, IEventManager eventManager) : base(customMessageManager, game, eventManager)
        {
        }

        public override IMapData Data => IMapData.Maps[MapTypes.Polus];

        public override Dictionary<int, bool> Doors { get; } = new Dictionary<int, bool>(12);

        public override float SpawnRadius => 1f;

        public override Vector2 InitialSpawnCenter { get; } = new Vector2(16.64f, -2.46f);

        public override Vector2 MeetingSpawnCenter { get; } = new Vector2(17.4f, -16.286f);

        public Vector2 MeetingSpawnCenter2 { get; } = new Vector2(17.4f, -17.515f);

        public override Vector2 GetSpawnLocation(InnerPlayerControl player, int numPlayers, bool initialSpawn)
        {
            if (initialSpawn)
            {
                return base.GetSpawnLocation(player, numPlayers, initialSpawn);
            }

            var halfPlayers = numPlayers / 2; // floored intentionally
            var spawnId = player.PlayerId % 15;
            if (player.PlayerId < halfPlayers)
            {
                return this.MeetingSpawnCenter + (new Vector2(0.6f, 0) * spawnId);
            }
            else
            {
                return this.MeetingSpawnCenter2 + (new Vector2(0.6f, 0) * (spawnId - halfPlayers));
            }
        }

        protected override void AddSystems(Dictionary<SystemTypes, ISystemType> systems)
        {
            base.AddSystems(systems);

            systems.Add(SystemTypes.Doors, new DoorsSystemType(Doors));
            systems.Add(SystemTypes.Comms, new HudOverrideSystemType(Game, EventManager));
            systems.Add(SystemTypes.Security, new SecurityCameraSystemType());
            systems.Add(SystemTypes.Laboratory, new ReactorSystemType(Game, EventManager));
        }
    }
}
