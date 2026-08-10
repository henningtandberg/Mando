using Mando.Command;
using Mando.Event;

namespace Mando.Example;

internal sealed class Application(IDispatcher dispatcher, IEventBus eventBus) : IApplication
{
    public async Task RunAsync()
    {
        // A CancellationToken can be forwarded to command handlers; here it is left uncancelled.
        using var cancellation = new CancellationTokenSource();
        await dispatcher.Dispatch(new DoSomethingCommand(), cancellation.Token);

        using (new OrderNotifierRegisteredAnyTimeDuringRuntime(eventBus))
        {
            // OrderNotifierRegisteredAnyTimeDuringRuntime and OrderNotifierRegisteredInDI both fire
            await eventBus.Publish(new OrderPlaced(1)); 
        }

        // Only OrderNotifierRegisteredInDI fires
        await eventBus.Publish(new OrderPlaced(2));
    }
}
