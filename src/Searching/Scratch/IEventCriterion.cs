namespace ScrubJay.Reflection.Searching.Scratch;

public interface IEventCriterion : IMemberCriterion<EventInfo>
{
    ICriterion<Type>? HandlerType { get; set; }
}

public record class EventCriterion : MemberCriterion<EventInfo>, IEventCriterion
{
    public ICriterion<Type>? HandlerType { get; set; }

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

        if (HandlerType is not null && !HandlerType.Matches(vent.EventHandlerType))
            return false;

        return true;
    }
}

public interface IEventCriterionBuilder<out TBuilder> : 
    IMemberCriterionBuilder<TBuilder, IEventCriterion, EventInfo>
    where TBuilder : IEventCriterionBuilder<TBuilder>
{
    TBuilder HandlerType(ICriterion<Type> criterion);
    TBuilder HandlerType(Type type, TypeMatch typeMatch = TypeMatch.Exact);
    TBuilder HandlerType<TEvent>(TypeMatch typeMatch = TypeMatch.Exact);
}

internal class EventCriterionBuilder<TBuilder> :
    MemberCriterionBuilder<TBuilder, IEventCriterion, EventInfo>,
    IEventCriterionBuilder<TBuilder>
    where TBuilder : IEventCriterionBuilder<TBuilder>
{
    protected EventCriterionBuilder(IEventCriterion criterion) : base(criterion)
    {
    }

    public TBuilder HandlerType(ICriterion<Type> criterion)
    {
        _criterion.HandlerType = criterion;
        return _builder;
    }
    public TBuilder HandlerType(Type type, TypeMatch typeMatch = TypeMatch.Exact) 
        => HandlerType(new TypeMatchCriterion(type, typeMatch));
    public TBuilder HandlerType<TField>(TypeMatch typeMatch = TypeMatch.Exact) 
        => HandlerType(new TypeMatchCriterion(typeof(TField), typeMatch));
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