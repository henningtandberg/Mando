namespace Mando.Example; 

internal sealed class Service : IService
{
    public Task DoSomething()
    {
        Console.WriteLine("Did something!");
        return Task.CompletedTask;
    }
}