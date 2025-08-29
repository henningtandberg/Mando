namespace Commander;

internal sealed class Dispatcher(IEnumerable<ICommandHandler> handlers) : IDispatcher
{
    public async Task Dispatch(ICommand command)
    {
        var handler = handlers.FirstOrDefault(h => CanHandle(h, command)) ??
                      throw new InvalidOperationException($"Could not find any handler for {command.GetType()}");

        await handler.Execute(command);
    }

    private static bool CanHandle(ICommandHandler commandHandler, ICommand command)
    {
        var handlerType = commandHandler.GetType();
        
        var handlerInterfaceWithGenericArgument = handlerType
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>));

        if (handlerInterfaceWithGenericArgument is null)
        {
            return false;
        }

        var genericArgumentType = handlerInterfaceWithGenericArgument
            .GetGenericArguments()
            .FirstOrDefault();

        return genericArgumentType is not null && genericArgumentType.IsInstanceOfType(command);
    }
}