using System.IO;
using System.Net;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;
using Impostor.Hazel;

namespace Impostor.Server.Net.State
{
    internal partial class Game : IGame
    {
        IClientPlayer? IGame.Host => Host;

        IGameNet IGame.GameNet => GameNet;

        public void BanIp(IPAddress ipAddress)
        {
            _bannedIps.Add(ipAddress);
        }

        public async ValueTask SyncSettingsAsync()
        {
            if (Host?.Character == null)
            {
                throw new ImpostorException("Attempted to set infected when the host was not spawned.");
            }

            using (var writer = StartRpc(Host.Character.NetId, RpcCalls.SyncSettings))
            {
                // Someone will probably forget to do this, so we include it here.
                // If this is not done, the host will overwrite changes later with the defaults.
                Options.IsDefaults = false;

                await using (var memory = new MemoryStream())
                await using (var writerBin = new BinaryWriter(memory))
                {
                    Options.Serialize(writerBin, GameOptionsData.LatestVersion);
                    writer.WriteBytesAndSize(memory.ToArray());
                }

                await FinishRpcAsync(writer);
            }
        }

        public async ValueTask SendSettingsToPlayerAsync(GameOptionsData options, IInnerPlayerControl player)
        {
            if (Host.Character == null)
            {
                throw new ImpostorException("Attempted to set infected when the host was not spawned.");
            }

            using (var writer = StartRpc(Host.Character.NetId, RpcCalls.SyncSettings))
            {
                await using (var memory = new MemoryStream())
                await using (var writerBin = new BinaryWriter(memory))
                {
                    options.Serialize(writerBin, GameOptionsData.LatestVersion);
                    writer.WriteBytesAndSize(memory.ToArray());
                }

                await FinishRpcAsync(writer, player.OwnerId);
            }
        }

        public async ValueTask SetPrivacyAsync(bool isPublic)
        {
            IsPublic = isPublic;

            using (var writer = MessageWriter.Get(MessageType.Reliable))
            {
                WriteAlterGameMessage(writer, false, IsPublic);

                await SendToAllAsync(writer);
            }
        }
    }
}
