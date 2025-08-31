using Mando.Tests.Setup;

namespace Mando.Tests.CommandHandlers;

public class AuditHandler(IStd std) :
    ICommandHandler<CreateUser>,
    ICommandHandler<UpdateUser>,
    ICommandHandler<DeleteUser>
{
    private readonly string _handlerName = nameof(AuditHandler);
    
    public Task Execute(CreateUser createUser)
    {
        std.Write($"User {createUser.Id} was created by {_handlerName}");
        return Task.CompletedTask;
    }

    public Task Execute(UpdateUser updateUser)
    {
        std.Write($"User {updateUser.Id} was updated by {_handlerName}");
        return Task.CompletedTask;
    }

    public Task Execute(DeleteUser deleteUser)
    {
        std.Write($"User {deleteUser.Id} was deleted by {_handlerName}");
        return Task.CompletedTask;
    }
}

public interface IUser
{
    public string Id { get; init; }
}
public sealed record CreateUser(string Id) : IUser, ICommand;
public sealed record UpdateUser(string Id) : IUser, ICommand;
public sealed record DeleteUser(string Id) : IUser, ICommand;
