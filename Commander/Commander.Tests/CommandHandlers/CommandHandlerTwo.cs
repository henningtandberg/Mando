using Commander.Tests.Commands;
using Commander.Tests.Setup;

namespace Commander.Tests.CommandHandlers;

public sealed class CommandHandlerTwo(IStd std) : ICommandHandler<CommandTwo>
{
    public Task Execute(CommandTwo command)
    {
        std.Write("Command two executed!");
        return Task.CompletedTask;
    }
    
}