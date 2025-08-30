namespace Commander.Example;

public class Service : IService
{
    public Task DoSomething()
    {
        Console.WriteLine("Did something!");
        return Task.CompletedTask;
    }
}