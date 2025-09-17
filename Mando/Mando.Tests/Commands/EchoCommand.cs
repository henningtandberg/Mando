namespace Mando.Tests.Commands;
using Mando;

public sealed class EchoCommand(string message) : ICommand<string>
{
    public string Message { get; } = message;
}
