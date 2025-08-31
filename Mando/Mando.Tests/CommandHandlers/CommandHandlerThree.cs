using Mando.Tests.Commands;
using Mando.Tests.Setup;

namespace Mando.Tests.CommandHandlers;

public sealed class CommandHandlerThree(IStd std) : ICommandHandler<CommandThree>
{
    public Task Execute(CommandThree command)
    {
        std.Write("Command three executed!");
        return Task.CompletedTask;
    }
}