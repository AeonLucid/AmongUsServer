using System;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Net.Messages;
using Impostor.Server.Events.System;

namespace Impostor.Server.Net.Inner.Objects.Systems
{
    public abstract class SystemType : ISystemType
    {
        private readonly IEventManager _eventManager;

        private readonly IGame _game;

        public SystemType(IGame game, IEventManager eventManager)
        {
            _game = game;
            _eventManager = eventManager;
        }

        protected IGame Game => _game;

        protected IEventManager EventManager => _eventManager;

        public virtual void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public virtual void Deserialize(IMessageReader reader, bool initialState)
        {
            throw new NotImplementedException();
        }
    }
}
