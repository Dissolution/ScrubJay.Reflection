namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class EventInfoExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventInfo"></param>
    /// <returns></returns>
    /// <see href="https://stackoverflow.com/questions/9847424/is-the-backing-field-of-a-compiler-generated-event-always-guaranteed-to-use-the"/>
    /// <remarks>
    /// This is **NOT** guaranteed to consistently work if the compiler team changes their minds.
    /// </remarks>
    public static FieldInfo? GetBackingField(this EventInfo? eventInfo)
    {
        if (eventInfo is null) return null;
        
        BF flags = BF.DeclaredOnly | BF.NonPublic;
        
        if (eventInfo.IsStatic())
        {
            flags |= BF.Static;
        }
        else
        {
            flags |= BF.Instance;
        }
        return eventInfo.OwnerType()?.GetField(eventInfo.Name, flags);
    }
}