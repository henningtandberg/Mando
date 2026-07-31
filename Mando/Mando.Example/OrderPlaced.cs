using Mando.Event;

namespace Mando.Example;

internal sealed record OrderPlaced(int OrderId) : IEvent;
