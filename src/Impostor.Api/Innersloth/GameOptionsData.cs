using System;
using System.IO;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Innersloth
{
    public class GameOptionsData
    {
        /// <summary>
        ///     The latest major version of the game client.
        /// </summary>
        public const int LatestVersion = 4;

        /// <summary>
        ///     Gets or sets host's version of the game.
        /// </summary>
        public byte Version { get; set; } = LatestVersion;

        /// <summary>
        ///     Gets or sets the maximum amount of players for this lobby.
        /// </summary>
        public byte MaxPlayers { get; set; } = 10;

        /// <summary>
        ///     Gets or sets the language of the lobby as per <see cref="GameKeywords" /> enum.
        /// </summary>
        public GameKeywords Keywords { get; set; } = GameKeywords.English;

        /// <summary>
        ///     Gets or sets the Map selected for this lobby.
        /// </summary>
        public MapTypes Map { get; set; } = MapTypes.Skeld;

        /// <summary>
        ///     Gets or sets the Player speed modifier.
        /// </summary>
        public float PlayerSpeedMod { get; set; } = 1f;

        /// <summary>
        ///     Gets or sets the Light modifier for the players that are members of the crew as a multiplier value.
        /// </summary>
        public float CrewLightMod { get; set; } = 1f;

        /// <summary>
        ///     Gets or sets the Light modifier for the players that are Impostors as a multiplier value.
        /// </summary>
        public float ImpostorLightMod { get; set; } = 1f;

        /// <summary>
        ///     Gets or sets the Impostor cooldown to kill in seconds.
        /// </summary>
        public float KillCooldown { get; set; } = 15f;

        /// <summary>
        ///     Gets or sets the number of common tasks.
        /// </summary>
        public int NumCommonTasks { get; set; } = 1;

        /// <summary>
        ///     Gets or sets the number of long tasks.
        /// </summary>
        public int NumLongTasks { get; set; } = 1;

        /// <summary>
        ///     Gets or sets the number of short tasks.
        /// </summary>
        public int NumShortTasks { get; set; } = 2;

        /// <summary>
        ///     Gets or sets the maximum amount of emergency meetings each player can call during the game in seconds.
        /// </summary>
        public int NumEmergencyMeetings { get; set; } = 1;

        /// <summary>
        ///     Gets or sets the cooldown between each time any player can call an emergency meeting in seconds.
        /// </summary>
        public int EmergencyCooldown { get; set; } = 15;

        /// <summary>
        ///     Gets or sets the number of impostors for this lobby.
        /// </summary>
        public int NumImpostors { get; set; } = 1;

        /// <summary>
        ///     Gets or sets a value indicating whether ghosts (dead crew members) can do tasks.
        /// </summary>
        public bool GhostsDoTasks { get; set; } = true;

        /// <summary>
        ///     Gets or sets the Kill as per values in <see cref="KillDistances" />.
        /// </summary>
        public KillDistances KillDistance { get; set; } = KillDistances.Normal;

        /// <summary>
        ///     Gets or sets the time for discussion before voting time in seconds.
        /// </summary>
        public int DiscussionTime { get; set; } = 15;

        /// <summary>
        ///     Gets or sets the time for voting in seconds.
        /// </summary>
        public int VotingTime { get; set; } = 120;

        /// <summary>
        ///     Gets or sets a value indicating whether an ejected player is an impostor or not.
        /// </summary>
        public bool ConfirmImpostor { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether players are able to see tasks being performed by other players.
        /// </summary>
        /// <remarks>
        ///     By being set to true, tasks such as Empty Garbage, Submit Scan, Clear asteroids, Prime shields execution will be visible to other players.
        /// </remarks>
        public bool VisualTasks { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether the vote is anonymous.
        /// </summary>
        public bool AnonymousVotes { get; set; }

        /// <summary>
        ///     Gets or sets the task bar update mode as per values in <see cref="Innersloth.TaskBarUpdate" />.
        /// </summary>
        public TaskBarUpdate TaskBarUpdate { get; set; } = TaskBarUpdate.Always;

        /// <summary>
        ///     Gets or sets a value indicating whether the GameOptions are the default ones.
        /// </summary>
        public bool IsDefaults { get; set; } = true;

        /// <summary>
        ///     Deserialize a packet/message to a new GameOptionsData object.
        /// </summary>
        /// <param name="reader">Message reader object containing the raw message.</param>
        /// <returns>GameOptionsData object.</returns>
        public static GameOptionsData DeserializeCreate(IMessageReader reader)
        {
            var options = new GameOptionsData();
            options.Deserialize(reader.ReadBytesAndSize());
            return options;
        }

        /// <summary>
        ///     Serializes this instance of GameOptionsData object to a specified BinaryWriter.
        /// </summary>
        /// <param name="writer">The stream to write the message to.</param>
        /// <param name="version">The version of the game.</param>
        public void Serialize(BinaryWriter writer, byte version = LatestVersion)
        {
            writer.Write((byte)version);
            writer.Write((byte)MaxPlayers);
            writer.Write((uint)Keywords);
            writer.Write((byte)Map);
            writer.Write((float)PlayerSpeedMod);
            writer.Write((float)CrewLightMod);
            writer.Write((float)ImpostorLightMod);
            writer.Write((float)KillCooldown);
            writer.Write((byte)NumCommonTasks);
            writer.Write((byte)NumLongTasks);
            writer.Write((byte)NumShortTasks);
            writer.Write((int)NumEmergencyMeetings);
            writer.Write((byte)NumImpostors);
            writer.Write((byte)KillDistance);
            writer.Write((uint)DiscussionTime);
            writer.Write((uint)VotingTime);
            writer.Write((bool)IsDefaults);

            if (version > 1)
            {
                writer.Write((byte)EmergencyCooldown);
            }

            if (version > 2)
            {
                writer.Write((bool)ConfirmImpostor);
                writer.Write((bool)VisualTasks);
            }

            if (version > 3)
            {
                writer.Write((bool)AnonymousVotes);
                writer.Write((byte)TaskBarUpdate);
            }

            if (version > 4)
            {
                throw new ImpostorException($"Unknown GameOptionsData version {Version}.");
            }
        }

        public void Serialize(IMessageWriter writer)
        {
            using var memory = new MemoryStream();
            using var writerBin = new BinaryWriter(memory);
            Serialize(writerBin);
            writer.WriteBytesAndSize(memory.ToArray());
        }

        /// <summary>
        ///     Deserialize a ReadOnlyMemory object to this instance of the GameOptionsData object.
        /// </summary>
        /// <param name="memory">Memory containing the message/packet.</param>
        public void Deserialize(ReadOnlyMemory<byte> memory)
        {
            var bytes = memory.Span;

            Version = bytes.ReadByte();
            MaxPlayers = bytes.ReadByte();
            Keywords = (GameKeywords)bytes.ReadUInt32();
            Map = (MapTypes)bytes.ReadByte();
            PlayerSpeedMod = bytes.ReadSingle();

            CrewLightMod = bytes.ReadSingle();
            ImpostorLightMod = bytes.ReadSingle();
            KillCooldown = bytes.ReadSingle();

            NumCommonTasks = bytes.ReadByte();
            NumLongTasks = bytes.ReadByte();
            NumShortTasks = bytes.ReadByte();

            NumEmergencyMeetings = bytes.ReadInt32();

            NumImpostors = bytes.ReadByte();
            KillDistance = (KillDistances)bytes.ReadByte();
            DiscussionTime = bytes.ReadInt32();
            VotingTime = bytes.ReadInt32();

            IsDefaults = bytes.ReadBoolean();

            if (Version > 1)
            {
                EmergencyCooldown = bytes.ReadByte();
            }

            if (Version > 2)
            {
                ConfirmImpostor = bytes.ReadBoolean();
                VisualTasks = bytes.ReadBoolean();
            }

            if (Version > 3)
            {
                AnonymousVotes = bytes.ReadBoolean();
                TaskBarUpdate = (TaskBarUpdate)bytes.ReadByte();
            }

            if (Version > 4)
            {
                throw new ImpostorException($"Unknown GameOptionsData version {Version}.");
            }
        }

        /// <summary>
        /// Create a shallow clone of the options.
        /// </summary>
        /// <returns>The cloned GameOptionsData.</returns>
        public GameOptionsData Clone()
        {
            return (GameOptionsData)MemberwiseClone();
        }
    }
}
