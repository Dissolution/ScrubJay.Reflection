namespace ScrubJay.Reflection.Extensions;

/// <summary>
/// Extensions related to <see cref="System.Reflection.NullabilityInfo"/>
/// </summary>
public static class NullabilityInfoExtensions
{
    private static readonly NullabilityInfoContext _nullabilityInfoContext = new();
    
    public static NullabilityInfo? NullabilityInfo(this ParameterInfo? parameter)
    {
        if (parameter is null)
            return null;

        try
        {
            return _nullabilityInfoContext.Create(parameter);
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    [return: NotNullIfNotNull(nameof(field))]
    public static NullabilityInfo? NullabilityInfo(this FieldInfo? field)
    {
        if (field is null)
            return null;
        return _nullabilityInfoContext.Create(field);
    }
    
    [return: NotNullIfNotNull(nameof(property))]
    public static NullabilityInfo? NullabilityInfo(this PropertyInfo? property)
    {
        if (property is null)
            return null;
        return _nullabilityInfoContext.Create(property);
    }
    
    [return: NotNullIfNotNull(nameof(@event))]
    public static NullabilityInfo? NullabilityInfo(this EventInfo? @event)
    {
        if (@event is null)
            return null;
        return _nullabilityInfoContext.Create(@event);
    }
    
    public static NullabilityInfo? NullabilityInfo(this MemberInfo? member)
    {
        return member switch
        {
            FieldInfo field => _nullabilityInfoContext.Create(field),
            PropertyInfo property => _nullabilityInfoContext.Create(property),
            EventInfo @event => _nullabilityInfoContext.Create(@event),
            _ => null,
        };
    }

    public static void Deconstruct(this NullabilityInfo? nullabilityInfo, 
        out NullabilityState readState,
        out NullabilityState writeState)
    {
        if (nullabilityInfo is not null)
        {
            readState = nullabilityInfo.ReadState;
            writeState = nullabilityInfo.WriteState;
        }
        else
        {
            readState = NullabilityState.Unknown;
            writeState = NullabilityState.Unknown;
        }
    }
   
    internal static (string? Prefix, string? Postfix) GetPrefixPostfix(this NullabilityInfo? nullabilityInfo)
    {
        if (nullabilityInfo is null)
            return default;

        var (read, write) = nullabilityInfo;

        // Attributes are usually declared as WriteMod, ReadMod
        return (write, read) switch
        {
            (NullabilityState.Unknown, NullabilityState.Unknown) => default,
            (NullabilityState.Unknown, NullabilityState.NotNull) => ("[NotNull]", null),
            (NullabilityState.Unknown, NullabilityState.Nullable) => ("[MaybeNull]", null),
            (NullabilityState.NotNull, NullabilityState.Unknown) => ("[DisallowNull]", null),
            (NullabilityState.NotNull, NullabilityState.NotNull) => default,
            (NullabilityState.NotNull, NullabilityState.Nullable) => ("[DisallowNull, MaybeNull]", null),
            (NullabilityState.Nullable, NullabilityState.Unknown) => ("[AllowNull]", null),
            (NullabilityState.Nullable, NullabilityState.NotNull) => ("[AllowNull, NotNull]", null),
            (NullabilityState.Nullable, NullabilityState.Nullable) => (null, "?"),
            _ => default,
        };
    }
}