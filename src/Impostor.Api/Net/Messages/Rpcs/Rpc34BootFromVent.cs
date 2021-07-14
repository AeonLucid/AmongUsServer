namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc34BootFromVent
    {
        public static void Serialize(IMessageWriter writer, int ventId)
        {
            writer.WritePacked(ventId);
        }

        public static void Deserialize(IMessageReader reader, out int ventId)
        {
            ventId = reader.ReadPackedInt32();
        }
    }
}
