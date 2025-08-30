namespace Commander.Example;

internal sealed class Application(IDispatcher dispatcher) : IApplication
{
    public Task RunAsync() => dispatcher.Dispatch(new DoSomethingCommand());
}