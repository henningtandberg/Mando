using System.Reflection;
using Commander.Tests.Commands;
using Commander.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;


namespace Commander.Tests;

public class SingleHandlerPerCommandTests
{
    private readonly FakeStd _std = new();
    private readonly IDispatcher _dispatcher;
    
    public SingleHandlerPerCommandTests()
    {
        _dispatcher = new ServiceCollection()
            .AddSingleton<IStd>(_std)
            .AddCommander(Assembly.GetExecutingAssembly())
            .BuildServiceProvider()
            .GetRequiredService<IDispatcher>();
    }
    
    [Fact]
    public async Task CommandOne_Dispatch_OnlyCommandHandlerOneIsExecuted()
    {
        await _dispatcher.Dispatch(new CommandOne());

        Assert.Single(_std.Out);
        Assert.Equal("Command one executed!", _std.Out.First());
    }
    
    [Fact]
    public async Task CommandTwo_Dispatch_OnlyCommandHandlerTwoIsExecuted()
    {
        await _dispatcher.Dispatch(new CommandTwo());

        Assert.Single(_std.Out);
        Assert.Equal("Command two executed!", _std.Out.First());
    }
    
    [Fact]
    public async Task CommandThree_Dispatch_OnlyCommandHandlerThreeIsExecuted()
    {
        await _dispatcher.Dispatch(new CommandThree());

        Assert.Single(_std.Out);
        Assert.Equal("Command three executed!", _std.Out.First());
    }
}