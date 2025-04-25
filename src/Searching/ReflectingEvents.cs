namespace ScrubJay.Reflection.Searching;

public sealed class ReflectingEvents : ReflectingEventInfos<ReflectingEvents>
{
    public ReflectingEvents(IEnumerable<EventInfo> members) 
        : base(members)
    {
    }
}

public abstract class ReflectingEventInfos<B> : ReflectingMemberBases<B, EventInfo>
    where B : ReflectingEventInfos<B>
{
    protected ReflectingEventInfos(IEnumerable<EventInfo> members) 
        : base(members)
    {
    }

    public B Handler(Type? handlerType, TypeMatch match = TypeMatch.Exact)
    {
        return Only(handlerType, match,
            static (field, ht, m) => field.EventHandlerType.Matches(ht, m));
    }

    public B Handler<D>(TypeMatch match = TypeMatch.Exact)
        where D : Delegate
        => Handler(typeof(D), match);
}