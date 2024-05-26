namespace ScrubJay.Reflection.Searching.Criteria;

public interface IEventCriterion : IMemberCriterion<EventInfo>
{
    ICriterion<Type> HandlerType { get; set; }
}

public record class EventCriterion : MemberCriterion<EventInfo>, IEventCriterion
{
    public ICriterion<Type> HandlerType { get; set; } = Criterion.Pass<Type>();

    public EventCriterion() : base() { }
    public EventCriterion(IMemberCriterion criterion) : base(criterion) { }
    public EventCriterion(IEventCriterion criterion) : base(criterion)
    {
        this.HandlerType = criterion.HandlerType;
    }
    
    public override bool Matches(EventInfo? vent)
    {
        if (!base.Matches(vent))
            return false;

        if (!HandlerType.Matches(vent.EventHandlerType))
            return false;

        return true;
    }
}

public interface IEventCriterionBuilder<out TBuilder> : 
    IMemberBaseCriterionBuilder<TBuilder, IEventCriterion, EventInfo>
    where TBuilder : IEventCriterionBuilder<TBuilder>
{
    TBuilder HandlerType(ICriterion<Type> type);
    TBuilder HandlerType(Type type, TypeMatch match = TypeMatch.Exact);
    TBuilder HandlerType<TEvent>(TypeMatch match = TypeMatch.Exact);
    
    TBuilder Like(EventInfo eventInfo);
}

internal class EventCriterionBuilder<TBuilder> :
    MemberCriterionBuilder<TBuilder, IEventCriterion, EventInfo>,
    IEventCriterionBuilder<TBuilder>
    where TBuilder : IEventCriterionBuilder<TBuilder>
{
    protected EventCriterionBuilder(IEventCriterion criterion) : base(criterion)
    {
    }

    public TBuilder HandlerType(ICriterion<Type> type)
    {
        _criterion.HandlerType = type;
        return _builder;
    }
    public TBuilder HandlerType(Type type, TypeMatch match = TypeMatch.Exact) 
        => HandlerType(Criterion.Match(type, match));
    public TBuilder HandlerType<TEvent>(TypeMatch match = TypeMatch.Exact) 
        => HandlerType(Criterion.Match(typeof(TEvent), match));

    public TBuilder Like(EventInfo eventInfo)
    {
        base.Like(eventInfo);
        _criterion.HandlerType = Criterion.Match(eventInfo.EventHandlerType);
        return _builder;
    }
}

public interface IEventCriterionBuilderImpl : 
    IEventCriterionBuilder<IEventCriterionBuilderImpl>;

internal class EventCriterionBuilderImpl : 
    EventCriterionBuilder<IEventCriterionBuilderImpl>,
    IEventCriterionBuilderImpl
{
    public EventCriterionBuilderImpl() : base(new EventCriterion()) { }
    public EventCriterionBuilderImpl(IMemberCriterion criterion) : base(new EventCriterion(criterion)) { }
    public EventCriterionBuilderImpl(IEventCriterion criterion) : base(criterion) { }
}