namespace ScrubJay.Reflection.Searching.Scratch;

public interface IEventCriterion : IMemberCriterion<EventInfo>
{
    ICriterion<Type>? HandlerType { get; set; }
}

public record class EventCriterion : MemberCriterion<EventInfo>, IEventCriterion
{
    public ICriterion<Type>? HandlerType { get; set; }

    public override bool Matches(EventInfo? vent)
    {
        if (!base.Matches(vent))
            return false;

        if (HandlerType is not null && !HandlerType.Matches(vent.EventHandlerType))
            return false;

        return true;
    }
}