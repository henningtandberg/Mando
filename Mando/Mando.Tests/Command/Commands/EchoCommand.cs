using Mando.Command;

namespace Mando.Tests.Command.Commands;

public sealed class EchoCommand(string message) : ICommand<string>
{
    public string Message { get; } = message;
}
