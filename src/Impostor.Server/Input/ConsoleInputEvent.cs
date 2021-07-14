using Impostor.Api.Events.Input;

namespace Impostor.Server.Input
{
    public class ConsoleInputEvent : IConsoleInputEvent
    {
        public ConsoleInputEvent(string input)
        {
            Input = input;
        }

        public string Input { get; }
    }
}
