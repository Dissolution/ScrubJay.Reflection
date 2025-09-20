namespace ScrubJay.Reflection.Utilities;

#if !NET6_0_OR_GREATER
public sealed class NullabilityInfo
{
    public NullabilityInfo(Type type,
        NullabilityState readState,
        NullabilityState writeState,
        NullabilityInfo? elementType,
        NullabilityInfo[] typeArguments)
    {
        Type = type;
        ReadState = readState;
        WriteState = writeState;
        ElementType = elementType;
        GenericTypeArguments = typeArguments;
    }

    /// <summary>
    /// The <see cref="System.Type" /> of the member or generic parameter
    /// to which this NullabilityInfo belongs
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// The nullability read state of the member
    /// </summary>
    public NullabilityState ReadState { get; }

    /// <summary>
    /// The nullability write state of the member
    /// </summary>
    public NullabilityState WriteState { get; }

    /// <summary>
    /// If the member type is an array, gives the <see cref="NullabilityInfo" /> of the elements of the array, null otherwise
    /// </summary>
    public NullabilityInfo? ElementType { get; }

    /// <summary>
    /// If the member type is a generic type, gives the array of <see cref="NullabilityInfo" /> for each type parameter
    /// </summary>
    public NullabilityInfo[] GenericTypeArguments { get; }
}

public enum NullabilityState
{
    /// <summary>
    /// Nullability context not enabled (oblivious)
    /// </summary>
    Unknown,

    /// <summary>
    /// Non nullable value or reference type
    /// </summary>
    NotNull,

    /// <summary>
    /// Nullable value or reference type
    /// </summary>
    Nullable,
}
#endif


[PublicAPI]
public static class Nullability
{
#if NET6_0_OR_GREATER
    private static readonly NullabilityInfoContext _context = new NullabilityInfoContext();
    
    public static NullabilityInfo? Get(EventInfo @event)
    {
        return _context.Create(@event);
    }
    
    public static NullabilityInfo? Get(FieldInfo field)
    {
        return _context.Create(field);
    }
    
    public static NullabilityInfo? Get(ParameterInfo parameter)
    {
        return _context.Create(parameter);
    }
    
    public static NullabilityInfo? Get(PropertyInfo property)
    {
        return _context.Create(property);
    }
    
    public static NullabilityInfo? Get(MemberInfo member)
    {
        if (member is FieldInfo field)
            return Get(field);
        if (member is PropertyInfo property)
            return Get(property);
        if (member is EventInfo @event)
            return Get(@event);
        return null;
    }

#else

    public static NullabilityInfo? Get(EventInfo @event)
    {
        return null;
    }

    public static NullabilityInfo? Get(FieldInfo field)
    {
        return null;
    }

    public static NullabilityInfo? Get(ParameterInfo parameter)
    {
        return null;
    }

    public static NullabilityInfo? Get(PropertyInfo property)
    {
        return null;
    }

    public static NullabilityInfo? Get(MemberInfo member)
    {
        if (member is FieldInfo field)
            return Get(field);
        if (member is PropertyInfo property)
            return Get(property);
        if (member is EventInfo @event)
            return Get(@event);
        return null;
    }

#endif

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

    public static (string? Prefix, string? Postfix) GetPrefixPostfix(this NullabilityInfo? nullabilityInfo)
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