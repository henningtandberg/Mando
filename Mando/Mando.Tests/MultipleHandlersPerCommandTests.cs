using System.Reflection;
using Mando.Tests.Commands;
using Mando.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Mando.Tests;

public class MultipleHandlersPerCommandTests
{
    private readonly FakeStd _std = new();
    private readonly IDispatcher _dispatcher;

    public MultipleHandlersPerCommandTests()
    {
        _dispatcher = new ServiceCollection()
            .AddSingleton<IStd>(_std)
            .AddCommander(Assembly.GetExecutingAssembly())
            .BuildServiceProvider()
            .GetRequiredService<IDispatcher>();
    }
    
    [Fact]
    public async Task OneCommand_Dispatch_TheCommandIsExecutedByAllHandlersRegisteredToHandleTheSpecifiedCommand()
    {
        await _dispatcher.Dispatch(new Order66());
        
        Assert.Equal(3, _std.Out.Count);
        Assert.Equal(
            [
                "Clone commando executed order 66!",
                "Clone gunner executed order 66!",
                "Clone paratrooper executed order 66!"
            ],
            _std.Out);
    }
}