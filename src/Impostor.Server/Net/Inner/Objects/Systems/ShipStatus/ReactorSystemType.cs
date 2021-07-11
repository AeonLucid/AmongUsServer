using System;
using System.Collections.Generic;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Net.Messages;
using Impostor.Server.Events.System;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class ReactorSystemType : SystemType, IActivatable
    {
        public ReactorSystemType(IGame game, IEventManager eventManager) : base(game, eventManager)
        {
            Countdown = 10000f;
            UserConsolePairs = new HashSet<Tuple<byte, byte>>();
        }

        public float Countdown { get; private set; }

        public HashSet<Tuple<byte, byte>> UserConsolePairs { get; }

        public bool IsActive => Countdown < 10000.0;

        public override void Deserialize(IMessageReader reader, bool initialState)
        {
            float value = reader.ReadSingle();

            if (IsActive ^ (value < 10000.0))
            {
                // throw event
            }

            Countdown = value;

            UserConsolePairs.Clear(); // TODO: Thread safety

            var count = reader.ReadPackedInt32();

            for (var i = 0; i < count; i++)
            {
                UserConsolePairs.Add(new Tuple<byte, byte>(reader.ReadByte(), reader.ReadByte()));
            }
        }
    }
}
