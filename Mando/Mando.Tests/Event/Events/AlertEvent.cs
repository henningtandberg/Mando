using Mando.Event;

namespace Mando.Tests.Event.Events;

public sealed record AlertEvent(string Message) : IEvent;
