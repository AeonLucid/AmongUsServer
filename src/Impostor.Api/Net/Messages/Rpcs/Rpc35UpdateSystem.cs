using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc35UpdateSystem
    {
        public static void Serialize(IMessageWriter writer, SystemTypes systemType, IInnerPlayerControl playerControl, ushort sid, byte state, byte ventId)
        {
            writer.Write((byte)systemType);
            writer.Write(playerControl.NetId);
            writer.Write(sid);
            writer.Write(state);
            writer.Write(ventId);
        }

        public static void Deserialize(IMessageReader reader, IGame game, out SystemTypes systemType, out IInnerPlayerControl? playerControl, out ushort sId, out byte state, out byte ventId)
        {
            systemType = (SystemTypes)reader.ReadByte();
            playerControl = reader.ReadNetObject<IInnerPlayerControl>(game);
            sId = reader.ReadUInt16();
            state = reader.ReadByte();
            ventId = reader.ReadByte();
        }
    }
}
