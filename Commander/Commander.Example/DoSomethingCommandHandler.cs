namespace Commander.Example;

internal sealed class DoSomethingCommandHandler(IService service) : ICommandHandler<DoSomethingCommand>
{
    public Task Execute(DoSomethingCommand command) => service.DoSomething();
}