using System;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Net.Messages;
using Impostor.Server.Events.System;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class HudOverrideSystemType : SystemType, IActivatable
    {
        public HudOverrideSystemType(IGame game, IEventManager eventManager) : base(game, eventManager)
        {
        }

        public bool IsActive { get; private set; }

        public override void Deserialize(IMessageReader reader, bool initialState)
        {
            bool status = reader.ReadBoolean();

            if (status != IsActive)
            {
                EventManager.CallAsync(new HudOverrideSystemEvent(Game, status));
            }

            IsActive = status;
        }
    }
}
