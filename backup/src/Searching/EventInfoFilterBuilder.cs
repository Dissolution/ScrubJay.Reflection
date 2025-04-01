namespace ScrubJay.Reflection.Searching;

public sealed class EventFilters : EventInfoFilterBuilder<EventFilters>
{
    public EventFilters(IEnumerable<EventInfo> events) : base(events)
    {
    }
}

public abstract class EventInfoFilterBuilder<TB> : MemberBaseFilterBuilder<TB, EventInfo>
    where TB : EventInfoFilterBuilder<TB>
{
    protected EventInfoFilterBuilder(IEnumerable<EventInfo> events) : base(events)
    {

    }

    public TB HandlerType(Type handlerType, TypeMatch match = TypeMatch.Exact)
        => Where(e => e.EventHandlerType.Equals(handlerType, match));
}
