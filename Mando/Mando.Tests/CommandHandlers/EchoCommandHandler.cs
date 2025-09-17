using Mando.Tests.Commands;

namespace Mando.Tests.CommandHandlers;

public sealed class EchoCommandHandler : ICommandHandler<EchoCommand, string>
{
    public Task<string> Execute(EchoCommand command)
    {
        return Task.FromResult(command.Message);
    }
}
