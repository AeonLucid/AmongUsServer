using Impostor.Api.Events.System;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.System
{
    public class HudOverrideSystemEvent : IHudOverrideSystemEvent
    {
        public HudOverrideSystemEvent(IGame game, bool active)
        {
            Game = game;

            IsActive = active;
        }

        public IGame Game { get; }

        public bool IsActive { get; }
    }
}
