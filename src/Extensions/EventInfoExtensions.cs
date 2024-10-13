using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection.Extensions;

/// <summary>
/// Extensions on <see cref="EventInfo"/>
/// </summary>
[PublicAPI]
public static class EventInfoExtensions
{
    /// <summary>
    /// Gets the backing <see cref="FieldInfo"/> for this <see cref="EventInfo"/>
    /// </summary>
    /// <param name="eventInfo"></param>
    /// <returns></returns>
    /// <see href="https://stackoverflow.com/questions/9847424/is-the-backing-field-of-a-compiler-generated-event-always-guaranteed-to-use-the"/>
    /// <remarks>
    /// This is **NOT** guaranteed to consistently work if the compiler team changes their minds
    /// </remarks>
    [return: NotNullIfNotNull(nameof(eventInfo))]
    public static FieldInfo? GetBackingField(this EventInfo? eventInfo)
    {
        if (eventInfo is null)
            return null;
        return Mirror.Search(eventInfo.DeclaringType!)
            .TryFindMember(b => b.IsField.NameFrom(eventInfo).AccessFrom(eventInfo)).OkOrThrow();
    }
}