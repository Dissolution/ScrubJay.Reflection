namespace ScrubJay.Reflection.Searching.Criteria;

public record class EventCriteria : MemberCriteria, ICriteria<EventInfo>
{
    public static new EventCriteria Create(MemberCriteria criteria)
    {
        return new()
        {
            Access = criteria.Access,
            MemberType = criteria.MemberType,
            Name = criteria.Name,
            Visibility = criteria.Visibility,
        };
    }


    public TypeCriteria? Type { get; set; } = null;
    
    // adder, remover, raiser

    public override MemberTypes MemberType => MemberTypes.Event;

    public bool Matches(EventInfo? vent)
    {
        if (!base.Matches(vent))
            return false;
        if (Type is not null && !Type.Matches(vent.EventHandlerType))
            return false;
        return true;
    }
}

public abstract class EventCriteriaBuilder<TBuilder, TCriteria> : MemberCriteriaBuilder<TBuilder, TCriteria>
    where TBuilder : MemberCriteriaBuilder<TBuilder, TCriteria>
    where TCriteria : EventCriteria, new()
{
    protected EventCriteriaBuilder() { }
    protected EventCriteriaBuilder(TCriteria criteria) : base(criteria) { }

    public TBuilder Type(TypeCriteria typeCriteria)
    {
        _criteria.Type = typeCriteria;
        return _builder;
    }
}

public sealed class EventCriteriaBuilder : EventCriteriaBuilder<EventCriteriaBuilder, EventCriteria>, ICriteria<EventInfo>
{
    internal EventCriteriaBuilder()
    {
    }
    internal EventCriteriaBuilder(EventCriteria criteria) : base(criteria)
    {
    }

    public bool Matches(EventInfo? vent) => _criteria.Matches(vent);
}