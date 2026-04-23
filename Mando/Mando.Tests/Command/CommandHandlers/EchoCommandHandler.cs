using Mando.Command;
using Mando.Tests.Command.Commands;

namespace Mando.Tests.Command.CommandHandlers;

public sealed class EchoCommandHandler : ICommandHandler<EchoCommand, string>
{
    public Task<string> Execute(EchoCommand command)
    {
        return Task.FromResult(command.Message);
    }
}
