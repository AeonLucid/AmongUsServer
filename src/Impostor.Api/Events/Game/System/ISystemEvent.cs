using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.System
{
    public interface ISystemEvent : IGameEvent
    {
        // /// <summary>
        // ///    Gets the <see cref="IInnerShipStatus" /> inside which that <see cref="ISystemEvent" /> happened
        // /// </summary>
        // IInnerShipStatus CurrentShip {get; };
    }
}
