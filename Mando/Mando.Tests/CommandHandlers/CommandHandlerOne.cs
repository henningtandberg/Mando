using Mando.Tests.Commands;
using Mando.Tests.Setup;

namespace Mando.Tests.CommandHandlers;

public sealed class CommandHandlerOne(IStd std) : ICommandHandler<CommandOne>
{
    public Task Execute(CommandOne command)
    {
        std.Write("Command one executed!");
        return Task.CompletedTask;
    }
}