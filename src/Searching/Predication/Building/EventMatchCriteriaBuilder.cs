using ScrubJay.Reflection.Searching.Predication.Members;

namespace ScrubJay.Reflection.Searching.Predication.Building;

public sealed class EventMatchCriteriaBuilder : EventMatchCriteriaBuilder<EventMatchCriteriaBuilder>
{
    public EventMatchCriteriaBuilder() : base()
    {
    }

    public EventMatchCriteriaBuilder(EventMatchCriteria criteria) : base(criteria)
    {
    }

    public EventMatchCriteriaBuilder(MemberMatchCriteria<MemberInfo> criteria) : base(criteria)
    {
    }
}

public class EventMatchCriteriaBuilder<TBuilder> : MemberMatchCriteriaBuilder<TBuilder, EventMatchCriteria, EventInfo>
    where TBuilder : EventMatchCriteriaBuilder<TBuilder>
{
    public EventMatchCriteriaBuilder() : base()
    {
    }

    public EventMatchCriteriaBuilder(EventMatchCriteria criteria) : base(criteria)
    {
    }

    public EventMatchCriteriaBuilder(MemberMatchCriteria<MemberInfo> criteria) : base(criteria)
    {
    }

    public TBuilder HandlerType(ICriteria<Type?> type)
    {
        _criteria.HandlerType = type;
        return _builder;
    }

    public TBuilder HandlerType(Type type, TypeMatchType match = TypeMatchType.Exact) => HandlerType(Criteria.Equals(type, match));
    public TBuilder HandlerType<TEvent>(TypeMatchType match = TypeMatchType.Exact) => HandlerType(Criteria.Equals<TEvent>(match));

    public TBuilder InheritFrom(EventInfo eventInfo)
    {
        base.InheritFrom(eventInfo);
        _criteria.HandlerType = Criteria.Equals(eventInfo.EventHandlerType);
        return _builder;
    }
}