using Impostor.Api.Innersloth;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerRepairSystemEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets the system that the player sabotaged.
        /// </summary>
        SystemTypes SystemType { get; }

        /// <summary>
        ///     Gets the amount that the system was sabotaged by.
        /// </summary>
        byte Amount { get; }
    }
}
