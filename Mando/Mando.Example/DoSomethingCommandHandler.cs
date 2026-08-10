using Mando.Command;

namespace Mando.Example;

internal sealed class DoSomethingCommandHandler(IService service) : ICommandHandler<DoSomethingCommand>
{
    public Task Execute(DoSomethingCommand command, CancellationToken cancellationToken) => service.DoSomething();
}