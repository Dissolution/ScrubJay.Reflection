namespace ScrubJay.Reflection.Searching.Predication.Members;

public sealed record class EventMatchCriteria : MemberMatchCriteria<EventInfo>
{
    public ICriteria<Type?>? HandlerType { get; set; }
    public ICriteria<MethodInfo?>? AddMethod { get; set; } 
    public ICriteria<MethodInfo?>? RemoveMethod { get; set; } 
    public ICriteria<MethodInfo?>? RaiseMethod { get; set; } 
    
    public override bool Matches(EventInfo? vent)
    {
        if (!base.Matches(vent))
            return false;

        if (!Matches(HandlerType, vent.EventHandlerType))
            return false;

        if (!Matches(AddMethod, vent.AddMethod))
            return false;
        
        if (!Matches(RemoveMethod, vent.RemoveMethod))
            return false;
        
        if (!Matches(RaiseMethod, vent.RaiseMethod))
            return false;

        return true;
    }
}