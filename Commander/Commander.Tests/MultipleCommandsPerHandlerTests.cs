using System.Reflection;
using Commander.Tests.CommandHandlers;
using Commander.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Commander.Tests;

public class MultipleCommandsPerHandlerTests 
{
    private readonly FakeStd _std = new();
    private readonly IDispatcher _dispatcher;

    public MultipleCommandsPerHandlerTests()
    {
        _dispatcher = new ServiceCollection()
            .AddSingleton<IStd>(_std)
            .AddCommander(Assembly.GetExecutingAssembly())
            .BuildServiceProvider()
            .GetRequiredService<IDispatcher>();
    }

    [Fact]
    public async Task MultipleCommands_Dispatch_AllCommandsAreHandledByTheSameHandler()
    {
        const string userId = "1234";

        await _dispatcher.Dispatch(new CreateUser(userId));
        await _dispatcher.Dispatch(new UpdateUser(userId));
        await _dispatcher.Dispatch(new DeleteUser(userId));

        var expected = new List<string> {
            "User 1234 was created by AuditHandler",
            "User 1234 was updated by AuditHandler",
            "User 1234 was deleted by AuditHandler"
        };
        Assert.Equal(3, _std.Out.Count);
        Assert.Equivalent(expected, _std.Out, strict: true);
    }
}