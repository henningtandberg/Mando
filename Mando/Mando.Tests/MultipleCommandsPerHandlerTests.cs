using System.Reflection;
using Mando.Tests.CommandHandlers;
using Mando.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Mando.Tests;

public class MultipleCommandsPerHandlerTests 
{
    private readonly FakeStd _std = new();
    private readonly IDispatcher _dispatcher;
    private readonly ServiceProvider _serviceProvider;

    public MultipleCommandsPerHandlerTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddSingleton<IStd>(_std)
            .AddMando(Assembly.GetExecutingAssembly())
            .BuildServiceProvider();
        
        _dispatcher = _serviceProvider.GetRequiredService<IDispatcher>();
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

    [Fact]
    public void MultipleCommands_SingleCommandHandler_DifferentInstanceReturnedForEachCommandHandler()
    {
        var createUserCommandHandler = _serviceProvider.GetRequiredService<ICommandHandler<CreateUser>>();
        var updateUserCommandHandler = _serviceProvider.GetRequiredService<ICommandHandler<UpdateUser>>();
        var deleteUserCommandHandler = _serviceProvider.GetRequiredService<ICommandHandler<DeleteUser>>();
        
        Assert.NotSame(createUserCommandHandler, updateUserCommandHandler);
        Assert.NotSame(createUserCommandHandler, deleteUserCommandHandler);
        Assert.NotSame(updateUserCommandHandler, deleteUserCommandHandler);
    }
}