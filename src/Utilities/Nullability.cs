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


/// <summary>
/// 
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis"/>
[PublicAPI]
public static class Nullability
{
    extension(NullabilityInfo? nullabilityInfo)
    {
        public void Deconstruct(out NullabilityState readState, out NullabilityState writeState)
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
    }


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

    public static (Attribute? ReadAttr, Attribute? WriteAttr) GetAttributes(MemberInfo member)
    {
        NullabilityInfo? nullabilityInfo = Get(member);
        if (nullabilityInfo is null)
            return default;
        
        // postconditions (on read) (get)
        Attribute? readAttr = nullabilityInfo.ReadState switch
        {
            NullabilityState.Nullable => new MaybeNullAttribute(),
            NullabilityState.NotNull => new NotNullAttribute(),
            _ => null,
        };

        // preconditions (on write) (set)
        Attribute? writeAttr = nullabilityInfo.WriteState switch
        {
            NullabilityState.Nullable => new AllowNullAttribute(),
            NullabilityState.NotNull => new DisallowNullAttribute(),
            _ => null,
        };
        return (readAttr, writeAttr);
    }
    
    // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
    public static (Attribute? ReadAttr, Attribute? WriteAttr, bool TypeNullable) GetConditions(PropertyInfo? property)
    {
        if (property is null)
        {
            return default;
        }

        NullabilityInfo? nullabilityInfo = Get(property);
        property.GetAttributes().TryGet<NullableAttribute>(out var nullableAttribute);
        var propertyType = property.PropertyType;
        propertyType.GetAttributes().TryGet<NullableContextAttribute>(out var nullableContextAttribute);

        if (nullabilityInfo is null && nullableContextAttribute is null)
        {
            return default;
        }


        NullableContext propertyContext = default;
        if (nullableAttribute is not null)
        {
            var flags = nullableAttribute.NullableFlags;
            if (flags.Length != 1 || flags[0] > 2)
                Debugger.Break();
            propertyContext = (NullableContext)flags[0];
        }

        NullableContext propertyTypeContext = (NullableContext)(nullableContextAttribute?.Flag ?? 0);
        var (readState, writeState) = nullabilityInfo;

        // postconditions (on read) (get)
        Attribute? readAttr = readState switch
        {
            NullabilityState.Nullable => new MaybeNullAttribute(),
            NullabilityState.NotNull => new NotNullAttribute(),
            _ => null,
        };

        // preconditions (on write) (set)
        Attribute? writeAttr = writeState switch
        {
            NullabilityState.Nullable => new AllowNullAttribute(),
            NullabilityState.NotNull => new DisallowNullAttribute(),
            _ => null,
        };

        if (propertyContext == NullableContext.Oblivious &&
            propertyTypeContext == NullableContext.Oblivious)
        {
            if (propertyType.IsValueType)
            {
                // we do not write any attributes nor q for value types
                return default;
            }
            else
            {
                Debugger.Break();
                throw new NotImplementedException();
            }
        }

        // the property context tells us if the return type has been specified as null
        bool typeNullable;
        if (propertyContext == NullableContext.Oblivious)
        {
            typeNullable = false;
        }
        else if (propertyContext == NullableContext.Annotated)
        {
            typeNullable = true;
        }
        else
        {
            Debugger.Break();
            throw new NotImplementedException();
        }


        if (!typeNullable)
        {
            if (readState == NullabilityState.NotNull)
                readAttr = null;
            if (writeState == NullabilityState.NotNull)
                writeAttr = null;
        }
        else
        {
            if (readState == NullabilityState.Nullable)
                readAttr = null;
            if (writeState == NullabilityState.Nullable)
                writeAttr = null;
        }

        return (readAttr, writeAttr, typeNullable);
    }
    

    public static (Attribute? ReadAttr, Attribute? WriteAttr, bool TypeNullable) GetConditions(MemberInfo? member)
    {
        if (member is null)
        {
            return default;
        }

        Type? relatedType = member switch
        {
            FieldInfo fieldInfo => fieldInfo.FieldType,
            PropertyInfo propertyInfo => propertyInfo.PropertyType,
            EventInfo eventInfo => eventInfo.EventHandlerType,
            MethodInfo methodInfo => methodInfo.ReturnType,
            ConstructorInfo constructorInfo => constructorInfo.DeclaringType,
            Type type => type,
            _ => null,
        };

        NullabilityInfo? nullabilityInfo = Get(member);
        NullableAttribute? nullableAttribute = null;
        NullableContextAttribute? nullableContextAttribute = null;
        
        member.GetAttributes().TryGet<NullableAttribute>(out nullableAttribute);
        relatedType?.GetAttributes().TryGet<NullableContextAttribute>(out nullableContextAttribute);

        if (nullabilityInfo is null && nullableContextAttribute is null)
        {
            return default;
        }
        
        NullableContext propertyContext = default;
        if (nullableAttribute is not null)
        {
            var flags = nullableAttribute.NullableFlags;
            if (flags.Length != 1 || flags[0] > 2)
                Debugger.Break();
            propertyContext = (NullableContext)flags[0];
        }

        NullableContext propertyTypeContext = (NullableContext)(nullableContextAttribute?.Flag ?? 0);
        var (readState, writeState) = nullabilityInfo;

        // postconditions (on read) (get)
        Attribute? readAttr = readState switch
        {
            NullabilityState.Nullable => new MaybeNullAttribute(),
            NullabilityState.NotNull => new NotNullAttribute(),
            _ => null,
        };

        // preconditions (on write) (set)
        Attribute? writeAttr = writeState switch
        {
            NullabilityState.Nullable => new AllowNullAttribute(),
            NullabilityState.NotNull => new DisallowNullAttribute(),
            _ => null,
        };

        // ?? no specification required ??
        if (propertyContext == NullableContext.Oblivious && propertyTypeContext == NullableContext.Oblivious)
        {
            return default;
        }

        // the property context tells us if the return type has been specified as null
        bool typeNullable = propertyContext switch
        {
            NullableContext.Oblivious => false,
            NullableContext.NotAnnotated => false,
            NullableContext.Annotated => true,
            _ => throw InvalidEnumException.New(propertyContext),
        };

        if (!typeNullable)
        {
            if (readState == NullabilityState.NotNull)
                readAttr = null;
            if (writeState == NullabilityState.NotNull)
                writeAttr = null;
        }
        else
        {
            if (readState == NullabilityState.Nullable)
                readAttr = null;
            if (writeState == NullabilityState.Nullable)
                writeAttr = null;
        }

        return (readAttr, writeAttr, typeNullable);
    }
    
      public static (Attribute? ReadAttr, Attribute? WriteAttr, bool TypeNullable) GetConditions(ParameterInfo? parameter)
    {
        if (parameter is null)
        {
            return default;
        }

        Type? relatedType = parameter.ParameterType;

        NullabilityInfo? nullabilityInfo = Get(parameter);
        NullableAttribute? nullableAttribute = null;
        NullableContextAttribute? nullableContextAttribute = null;
        
        parameter.GetAttributes().TryGet<NullableAttribute>(out nullableAttribute);
        relatedType?.GetAttributes().TryGet<NullableContextAttribute>(out nullableContextAttribute);

        if (nullabilityInfo is null && nullableContextAttribute is null)
        {
            return default;
        }
        
        NullableContext propertyContext = default;
        if (nullableAttribute is not null)
        {
            var flags = nullableAttribute.NullableFlags;
            if (flags.Length != 1 || flags[0] > 2)
                Debugger.Break();
            propertyContext = (NullableContext)flags[0];
        }

        NullableContext propertyTypeContext = (NullableContext)(nullableContextAttribute?.Flag ?? 0);
        var (readState, writeState) = nullabilityInfo;

        // postconditions (on read) (get)
        Attribute? readAttr = readState switch
        {
            NullabilityState.Nullable => new MaybeNullAttribute(),
            NullabilityState.NotNull => new NotNullAttribute(),
            _ => null,
        };

        // preconditions (on write) (set)
        Attribute? writeAttr = writeState switch
        {
            NullabilityState.Nullable => new AllowNullAttribute(),
            NullabilityState.NotNull => new DisallowNullAttribute(),
            _ => null,
        };

        // ?? no specification required ??
        if (propertyContext == NullableContext.Oblivious && propertyTypeContext == NullableContext.Oblivious)
        {
            return default;
        }

        // the property context tells us if the return type has been specified as null
        bool typeNullable = propertyContext switch
        {
            NullableContext.Oblivious => false,
            NullableContext.NotAnnotated => false,
            NullableContext.Annotated => true,
            _ => throw InvalidEnumException.New(propertyContext),
        };

        if (!typeNullable)
        {
            if (readState == NullabilityState.NotNull)
                readAttr = null;
            if (writeState == NullabilityState.NotNull)
                writeAttr = null;
        }
        else
        {
            if (readState == NullabilityState.Nullable)
                readAttr = null;
            if (writeState == NullabilityState.Nullable)
                writeAttr = null;
        }

        return (readAttr, writeAttr, typeNullable);
    }
}