namespace ScrubJay.Reflection.Text;

public static class TextBuilderMemberAppendExtensions
{
    internal static TBuilder AppendNullability<TBuilder>(this TBuilder text, NullabilityInfo? nullabilityInfo)
        where TBuilder : TextBuilderBase<TBuilder>
    {
        if (nullabilityInfo is null)
            return text;

        NullabilityState readState = nullabilityInfo.ReadState;
        NullabilityState writeState = nullabilityInfo.WriteState;

        return (readState, writeState) switch
        {
            (NullabilityState.Unknown, NullabilityState.Unknown) => text, // append nothing
            (NullabilityState.Unknown, NullabilityState.NotNull) => throw new NotImplementedException(),
            (NullabilityState.Unknown, NullabilityState.Nullable) => throw new NotImplementedException(),
            (NullabilityState.NotNull, NullabilityState.Unknown) => throw new NotImplementedException(),
            (NullabilityState.NotNull, NullabilityState.NotNull) => text, // append nothing
            (NullabilityState.NotNull, NullabilityState.Nullable) => throw new NotImplementedException(),
            (NullabilityState.Nullable, NullabilityState.Unknown) => throw new NotImplementedException(),
            (NullabilityState.Nullable, NullabilityState.NotNull) => throw new NotImplementedException(),
            (NullabilityState.Nullable, NullabilityState.Nullable) => text.Append('?'),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
    
    
    
    public static TBuilder AppendField<TBuilder>(this TBuilder text, FieldInfo? field)
        where TBuilder : TextBuilderBase<TBuilder>
    {
        if (field is null)
            return text;
        return text
            .AppendIf(field.IsStatic, "static ")
            .AppendType(field.FieldType)
            .AppendNullability(field.NullabilityInfo())
            .Append(' ')
            .AppendType(field.OwnerType())
            .Append('.')
            .Append(field.Name);
    }
    
    public static TBuilder AppendProperty<TBuilder>(this TBuilder text, PropertyInfo? property)
        where TBuilder : TextBuilderBase<TBuilder>
    {
        if (property is null)
            return text;
        return text 
            .AppendIf(property.IsStatic(), "static ")
            .AppendType(property.PropertyType)
            .AppendNullability(property.NullabilityInfo())
            .Append(' ')
            .AppendType(property.OwnerType())
            .Append('.')
            .Append(property.Name)
            .If(Validate.IsNotEmpty(property.GetIndexParameters()),
                static (tb, indexers) => tb.Append('[').Delimit(", ", indexers,
                    static (t, pi) => t.AppendParameter(pi)).Append(']'));
    }

    public static TBuilder AppendEvent<TBuilder>(this TBuilder text, EventInfo? @event)
        where TBuilder : TextBuilderBase<TBuilder>
    {
        if (@event is null)
            return text;
        return text
            .AppendIf(@event.IsStatic(), "static ")
            .AppendType(@event.EventHandlerType)
            .AppendNullability(@event.NullabilityInfo())
            .Append(' ')
            .AppendType(@event.OwnerType())
            .Append('.')
            .Append(@event.Name);
    }
    
    
    
    public static B AppendMethod<B>(this B builder, MethodBase? method)
        where B : TextBuilderBase<B>
    {
        if (method is null)
            return builder;

        ParameterInfo? returnParameter = method is MethodInfo methodInfo ? methodInfo.ReturnParameter : null;

        return builder
            .AppendIf(method.IsAsync(), "async ")
            .If(Validate.IsNotNull(returnParameter),
                static (tb, returnParam) => tb.AppendParameter(returnParam).Append(' '))
            .If(method, static m => m.DeclaringType is not null,
                static (tb, m) =>
                {
                    switch (m.Name)
                    {
                        case ".ctor":
                            tb.Append("new ").AppendType(m.DeclaringType);
                            break;
                        case ".cctor":
                            tb.Append("static ").AppendType(m.DeclaringType);
                            break;
                        default:
                            tb.AppendType(m.DeclaringType)
                                .Append(".")
                                .Append(m.Name);
                            break;
                    }
                },
                static (tb, m) => tb.Append(m.Name))
            .If(method.IsGenericMethod,
                tb => tb.Append('<').Delimit(", ", method.GetGenericArguments(), static (t, a) => t.AppendType(a)).Append('>'))
            .Append('(')
            .Delimit(", ", method.GetParameters(), static (tb, param) => AppendParameter(tb, param))
            .Append(')');
    }

    public static B AppendParameter<B>(this B builder, ParameterInfo? parameter)
        where B : TextBuilderBase<B>
    {
        if (parameter is null)
            return builder;
        var (paramRef, paramType) = parameter;
        return builder
            .Append(paramRef.AsString())
            .AppendType(paramType)
            .AppendNullability(parameter.NullabilityInfo())
            .If(Validate.IsNotEmpty(parameter.Name),
                static (tb, name) => tb.Append(' ').Append(name))
            .If(parameter.Default(),
                static (tb, defaultValue) => tb.Append(" = ").Append(defaultValue));
    }

    private static readonly TypeMap<string> _shortNames = new()
    {
        { typeof(bool), "bool" },
        { typeof(char), "char" },
        { typeof(sbyte), "sbyte" },
        { typeof(byte), "byte" },
        { typeof(short), "short" },
        { typeof(ushort), "ushort" },
        { typeof(int), "int" },
        { typeof(uint), "uint" },
        { typeof(long), "long" },
        { typeof(ulong), "ulong" },
        { typeof(float), "float" },
        { typeof(double), "double" },
        { typeof(decimal), "decimal" },
        { typeof(string), "string" },
        { typeof(object), "object" },
        { typeof(void), "void" },
        { typeof(nint), "nint" },
        { typeof(nuint), "nuint" },
    };

    public static B AppendType<B>(this B builder, Type? type)
        where B : TextBuilderBase<B>
    {
        if (type is null)
            return builder;

        string? name;
        Type? underType;

        if (_shortNames.TryGetValue(type, out name))
            return builder.Append(name);

        // Nullable is `T?`
        underType = Nullable.GetUnderlyingType(type);
        if (underType is not null)
        {
            return AppendType<B>(builder, underType).Append('?');
        }

        // Array is `T[,,,n]`
        if (type.IsArray)
        {
            underType = type.GetElementType()!;
            Debug.Assert(underType is not null);
            return AppendType<B>(builder, underType)
                .Append('[')
                .Repeat(type.GetArrayRank() - 1, ',')
                .Append(']');
        }

        // Pointers are `T*`
        if (type.IsPointer)
        {
            underType = type.GetElementType()!;
            Debug.Assert(underType is not null);
            return AppendType<B>(builder, underType).Append('*');
        }

        // Refs are `ref T` (could also be `&T`
        if (type.IsByRef)
        {
            underType = type.GetElementType()!;
            Debug.Assert(underType is not null);
            return builder.Append("ref ").AppendType(underType);
        }

        // Nested types we want to indicate their parent (but not generic parameters)
        if (type.IsNested && !type.IsGenericParameter)
        {
            builder.AppendType(type.DeclaringType).Append('.');
        }

        // If we are not generic, we append the name and are done
        if (!type.IsGenericType)
            return builder.Append(type.Name);

        Debug.Assert(!type.IsEnum);

        name = type.Name;
#if NETFRAMEWORK || NETSTANDARD2_0
        int index = name.IndexOf('`');
#else
        int index = name.IndexOf('`', StringComparison.Ordinal);
#endif
        if (index >= 0)
        {
            builder.Append(name.AsSpan(0, index));
        }
        else
        {
            builder.Append(name);
        }

        Type[] genericArguments = type.GetGenericArguments();
        return builder.Append('<')
            .Delimit<Type>(", ", genericArguments,
                (tb, argType) => tb.AppendType(argType))
            .Append('>');
    }
    
    public static B AppendMember<B>(this B builder, MemberInfo? member)
        where B : TextBuilderBase<B>
    {
        return member switch
        {
            null => builder.Append("null"),
            FieldInfo field => AppendField(builder, field),
            PropertyInfo property => AppendProperty(builder, property),
            EventInfo @event => AppendEvent(builder, @event),
            MethodBase method => AppendMethod(builder, method),
            Type type => AppendType(builder, type),
            _ => throw new NotImplementedException(),
        };
    }
}