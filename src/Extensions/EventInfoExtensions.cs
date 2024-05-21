using ScrubJay.Reflection.Searching;
using ScrubJay.Reflection.Searching.Criteria;

namespace ScrubJay.Reflection.Extensions;

public static class EventInfoExtensions
{
    /// <summary>
    /// Gets the backing <see cref="FieldInfo"/> for this <see cref="EventInfo"/>
    /// </summary>
    /// <param name="eventInfo"></param>
    /// <returns></returns>
    /// <see href="https://stackoverflow.com/questions/9847424/is-the-backing-field-of-a-compiler-generated-event-always-guaranteed-to-use-the"/>
    /// <remarks>
    /// This is **NOT** guaranteed to consistently work if the compiler team changes their minds.
    /// </remarks>
    [return: NotNullIfNotNull(nameof(eventInfo))]
    public static FieldInfo? GetBackingField(this EventInfo? eventInfo)
    {
        if (eventInfo is null)
            return null;

        MemberCriteria mc = new MemberCriteria();
        
        
        BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic;
        
        if (eventInfo.IsStatic())
        {
            flags |= BindingFlags.Static;
        }
        else
        {
            flags |= BindingFlags.Instance;
        }
        return eventInfo.DeclaringType?.GetField(eventInfo.Name, flags);
    }
}