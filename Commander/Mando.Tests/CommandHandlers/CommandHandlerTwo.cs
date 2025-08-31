using Mando.Tests.Commands;
using Mando.Tests.Setup;

namespace Mando.Tests.CommandHandlers;

public sealed class CommandHandlerTwo(IStd std) : ICommandHandler<CommandTwo>
{
    public Task Execute(CommandTwo command)
    {
        std.Write("Command two executed!");
        return Task.CompletedTask;
    }
    
}