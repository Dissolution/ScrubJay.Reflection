namespace ScrubJay.Reflection.Searching;

public sealed class MirrorEvents : MirrorEventsBuilder<MirrorEvents>
{
    internal MirrorEvents(Type reflectedType, IEnumerable<EventInfo> members) 
        : base(reflectedType, members)
    {
    }
}

public abstract class MirrorEventsBuilder<B> : MirrorMemberBaseBuilder<B, EventInfo>
    where B : MirrorEventsBuilder<B>
{
    protected MirrorEventsBuilder(Type reflectedType, IEnumerable<EventInfo> members) 
        : base(reflectedType, members)
    {
    }

    public B Handler(Type? handlerType, TypeMatch match = TypeMatch.Exact)
    {
        return Where(field => field.EventHandlerType.Matches(handlerType, match));
    }

    public B Handler<D>(TypeMatch match = TypeMatch.Exact)
        where D : Delegate
        => Handler(typeof(D), match);
}