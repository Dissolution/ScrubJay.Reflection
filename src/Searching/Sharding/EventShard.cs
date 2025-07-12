
using ScrubJay.Reflection.Collections;

namespace ScrubJay.Reflection.Searching.Sharding;


public abstract class EventShard<S> : MemberShard<S, EventInfo>
    where S : EventShard<S>
{
    protected EventShard(DLCL<EventInfo> events, List<string> filters) : base(events, filters)
    {
    }
    
    public S Handler(Type? handlerType, TypeMatch match = TypeMatch.Exact)
    {
        return AddFilter(e => e.EventHandlerType.Matches(handlerType, match),
            $"EventHandlerType {match:@} {handlerType:@}");
    }

    public S Handler<D>(TypeMatch match = TypeMatch.Exact)
        where D : Delegate
        => Handler(typeof(D), match);
 
}

[PublicAPI]
public sealed class EventShard : EventShard<EventShard>
{
    internal EventShard(DLCL<EventInfo> events, List<string> filters) : base(events, filters)
    {
    }
}