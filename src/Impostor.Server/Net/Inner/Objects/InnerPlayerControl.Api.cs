using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Inner.Objects.Components;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Server.Events.Player;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerPlayerControl : IInnerPlayerControl
    {
        IInnerPlayerPhysics IInnerPlayerControl.Physics => Physics;

        IInnerCustomNetworkTransform IInnerPlayerControl.NetworkTransform => NetworkTransform;

        IInnerPlayerInfo IInnerPlayerControl.PlayerInfo => PlayerInfo;

        public async ValueTask SetNameAsync(string name)
        {
            PlayerInfo.PlayerName = name;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetName);
            writer.Write(name);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SendNameToPlayerAsync(string name, IInnerPlayerControl? player = null)
        {
            if (player == null)
            {
                player = this;
            }

            using var writer = Game.StartRpc(NetId, RpcCalls.SetName);
            writer.Write(name);
            await Game.FinishRpcAsync(writer, player.OwnerId);
        }

        public async ValueTask SetColorAsync(ColorType color)
        {
            PlayerInfo.Color = color;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetColor);
            Rpc08SetColor.Serialize(writer, color);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SendColorToPlayerAsync(ColorType color, IInnerPlayerControl? player = null)
        {
            if (player == null)
            {
                player = this;
            }

            using var writer = Game.StartRpc(NetId, RpcCalls.SetColor);
            Rpc08SetColor.Serialize(writer, color);
            await Game.FinishRpcAsync(writer, player.OwnerId);
        }

        public async ValueTask SetHatAsync(HatType hat)
        {
            PlayerInfo.Hat = hat;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetHat);
            Rpc09SetHat.Serialize(writer, hat);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SendHatToPlayerAsync(HatType hat, IInnerPlayerControl? player = null)
        {
            if (player == null)
            {
                player = this;
            }

            using var writer = Game.StartRpc(NetId, RpcCalls.SetHat);
            Rpc09SetHat.Serialize(writer, hat);
            await Game.FinishRpcAsync(writer, player.OwnerId);
        }

        public async ValueTask SetPetAsync(PetType pet)
        {
            PlayerInfo.Pet = pet;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetPet);
            Rpc17SetPet.Serialize(writer, pet);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SendPetToPlayerAsync(PetType pet, IInnerPlayerControl? player = null)
        {
            if (player == null)
            {
                player = this;
            }

            using var writer = Game.StartRpc(NetId, RpcCalls.SetPet);
            Rpc17SetPet.Serialize(writer, pet);
            await Game.FinishRpcAsync(writer, player.OwnerId);
        }

        public async ValueTask SetSkinAsync(SkinType skin)
        {
            PlayerInfo.Skin = skin;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetSkin);
            Rpc10SetSkin.Serialize(writer, skin);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SendSkinToPlayerAsync(SkinType skin, IInnerPlayerControl? player = null)
        {
            if (player == null)
            {
                player = this;
            }

            using var writer = Game.StartRpc(NetId, RpcCalls.SetSkin);
            Rpc10SetSkin.Serialize(writer, skin);
            await Game.FinishRpcAsync(writer, player.OwnerId);
        }

        public async ValueTask SendChatAsync(string text)
        {
            using var writer = Game.StartRpc(NetId, RpcCalls.SendChat);
            writer.Write(text);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SendChatToPlayerAsync(string text, IInnerPlayerControl? player = null)
        {
            if (player == null)
            {
                player = this;
            }

            using var writer = Game.StartRpc(NetId, RpcCalls.SendChat);
            writer.Write(text);
            await Game.FinishRpcAsync(writer, player.OwnerId);
        }

        public async ValueTask MurderPlayerAsync(IInnerPlayerControl target)
        {
            if (!PlayerInfo.IsImpostor)
            {
                throw new ImpostorProtocolException("Tried to murder a player, but murderer was not the impostor.");
            }

            if (PlayerInfo.IsDead)
            {
                throw new ImpostorProtocolException("Tried to murder a player, but murderer was not alive.");
            }

            if (target.PlayerInfo.IsDead)
            {
                throw new ImpostorProtocolException("Tried to murder a player, but target was not alive.");
            }

            ((InnerPlayerControl)target).Die(DeathReason.Kill);

            using var writer = Game.StartRpc(NetId, RpcCalls.MurderPlayer);
            Rpc12MurderPlayer.Serialize(writer, target);
            await Game.FinishRpcAsync(writer);

            await _eventManager.CallAsync(new PlayerMurderEvent(Game, Game.GetClientPlayer(OwnerId)!, this, target));
        }

        public async ValueTask ExileAsync()
        {
            if (PlayerInfo.IsDead)
            {
                throw new ImpostorProtocolException("Tried to exile a player, but target was not alive.");
            }

            // Update player.
            Die(DeathReason.Exile);

            // Send RPC.
            using var writer = Game.StartRpc(NetId, RpcCalls.Exiled);
            Rpc04Exiled.Serialize(writer);
            await Game.FinishRpcAsync(writer);

            // Notify plugins.
            await _eventManager.CallAsync(new PlayerExileEvent(Game, Game.GetClientPlayer(OwnerId)!, this));
        }
    }
}
