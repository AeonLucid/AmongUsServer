namespace Impostor.Api.Events.Input
{
    public interface IConsoleInputEvent : IEvent
    {
        string Input { get; }
    }
}
