using System.Reflection;
using Commander;
using Commander.Example;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services =>
        services
            .AddSingleton<IService, Service>()
            .AddSingleton<IApplication, Application>()
            .AddCommander(Assembly.GetExecutingAssembly()))
    .Build();

using var scope = host.Services.CreateScope();

try
{
    await scope
        .ServiceProvider
        .GetRequiredService<IApplication>()
        .RunAsync();
}
catch (Exception e)
{
    Console.Error.WriteLine(e.Message);
}
finally
{
    host.Dispose(); 
}