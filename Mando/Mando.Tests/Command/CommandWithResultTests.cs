using System.Reflection;
using Mando.Command;
using Mando.Tests.Command.Commands;
using Mando.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Mando.Tests.Command;

public class CommandWithResultTests
{
    private readonly IDispatcher _dispatcher;

    public CommandWithResultTests()
    {
        _dispatcher = new ServiceCollection()
            .AddSingleton<IStd, FakeStd>()
            .AddMando(Assembly.GetExecutingAssembly())
            .BuildServiceProvider()
            .GetRequiredService<IDispatcher>();
    }

    [Fact]
    public async Task EchoCommand_ReturnsMessage()
    {
        string result = await _dispatcher.Dispatch(new EchoCommand("hello mando"));
        Assert.Equal("hello mando", result);
    }
}
