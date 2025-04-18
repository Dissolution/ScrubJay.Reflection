namespace ScrubJay.Reflection.Text;

public static class TextBuilderMemberAppendExtensions
{
    public static TBuilder AppendField<TBuilder>(this TBuilder text, FieldInfo? field)
        where TBuilder : TextBuilderBase<TBuilder>
    {
        if (field is null)
            return text;

        var (prefix, postfix) = field.NullabilityInfo().GetPrefixPostfix();

        return text
            .IfNotNull(prefix, static (tb, pf) => tb.Append(pf).Append(' '))
            .AppendIf(field.IsStatic, "static ")
            .AppendType(field.FieldType)
            .Append(postfix)
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

        var (prefix, postfix) = property.NullabilityInfo().GetPrefixPostfix();

        return text
            .IfNotNull(prefix, static (tb, pf) => tb.Append(pf).Append(' '))
            .AppendIf(property.IsStatic(), "static ")
            .AppendType(property.PropertyType)
            .Append(postfix)
            .Append(' ')
            .AppendType(property.OwnerType())
            .Append('.')
            .Append(property.Name)
            .IfNotEmpty(property.GetIndexParameters(),
                static (tb, indexers) => tb.Append('[').Delimit(", ", indexers,
                    static (t, pi) => t.AppendParameter(pi)).Append(']'));
    }

    public static TBuilder AppendEvent<TBuilder>(this TBuilder text, EventInfo? @event)
        where TBuilder : TextBuilderBase<TBuilder>
    {
        if (@event is null)
            return text;

        var (prefix, postfix) = @event.NullabilityInfo().GetPrefixPostfix();

        return text
            .IfNotNull(prefix, static (tb, pf) => tb.Append(pf).Append(' '))
            .AppendIf(@event.IsStatic(), "static ")
            .AppendType(@event.EventHandlerType)
            .Append(postfix)
            .Append(' ')
            .AppendType(@event.OwnerType())
            .Append('.')
            .Append(@event.Name);
    }

    public static B AppendConstructor<B>(this B builder, ConstructorInfo? ctor)
        where B : TextBuilderBase<B>
    {
        if (ctor is null) return builder;

        Type constructedType = ctor.DeclaringType.ThrowIfNull("Constructor has null Declaring Type");
        string name = constructedType.Name;
        if (name == ".ctor")
        {
            builder.Append("new ").AppendType(constructedType);
        }
        else if (name == ".cctor")
        {
            builder.Append("static ").AppendType(constructedType);
        }
        else
        {
            builder.Append("new ").AppendType(constructedType);
        }
        
        if (ctor.IsGenericMethod)
            Debugger.Break();

        return AppendParameters(builder, ctor.GetParameters());
    }
    
    public static B AppendMethodInfo<B>(this B builder, MethodInfo? method)
        where B : TextBuilderBase<B>
    {
        if (method is null) return builder;
        return builder
            .AppendIf(method.IsAsync(), "async ")
            .IfNotNull(method.ReturnParameter,
                static (tb, returnParam) => tb.AppendParameter(returnParam).Append(' '))
            .AppendType(method.OwnerType())
            .Append('.')
            .AppendNameAndGenericTypes(method.Name, method.GetGenericArguments())
            .AppendParameters(method.GetParameters());
    }

    public static B AppendParameter<B>(this B builder, ParameterInfo? parameter)
        where B : TextBuilderBase<B>
    {
        if (parameter is null)
            return builder;
        var (paramRef, paramType) = parameter;
        var (prefix, postfix) = parameter.NullabilityInfo().GetPrefixPostfix();
        return builder
            .IfNotEmpty(prefix, static (tb, pf) => tb.Append(pf).Append(' '))
            .Append(paramRef.AsString())
            .AppendType(paramType)
            .Append(postfix)
            .IfNotEmpty(parameter.Name, static (tb, name) => tb.Append(' ').Append(name))
            .If(parameter.Default(),
                static (tb, defaultValue) => tb.Append(" = ").Render(defaultValue));
    }

    public static B AppendAttribute<B>(this B builder, Attribute? attribute)
        where B : TextBuilderBase<B>
    {
        if (attribute is null)
            return builder;

        var str = attribute.ToString();
        return builder.Append(str);
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

        // If we are not generic or are an enum, we append the name only
        if (!type.IsGenericType || type.IsEnum)
            return builder.Append(type.Name);

        return AppendNameAndGenericTypes(builder, type.Name, type.GetGenericArguments());
    }

    internal static B AppendNameAndGenericTypes<B>(this B builder, string? name, Type[]? genericTypes)
        where B : TextBuilderBase<B>
    {
        int index = name?.IndexOf('`') ?? -1;
        return builder
            .If((name, index), static t => t.index >= 0,
            static (tb,t) => tb.Append(t.name.AsSpan(0, t.index)),
            static (tb, t) => tb.Append(t.name))
            .IfNotEmpty(genericTypes,
                static (tb, types) => tb
                    .Append('<')
                    .Delimit<Type>(", ", types, static (t, type) => t.AppendType(type))
                    .Append('>'));
    }
    
    internal static B AppendParameters<B>(this B builder, ParameterInfo[]? parameters)
        where B : TextBuilderBase<B>
        => builder.IfNotNull(parameters,
            static (tb, paramz) => tb.Append('(')
                .Delimit(", ", paramz, static (tb, param) => tb.AppendParameter(param))
                .Append(')'));


    public static B AppendMember<B>(this B builder, MemberInfo? member)
        where B : TextBuilderBase<B>
    {
        return member switch
        {
            null => builder.Append("null"),
            FieldInfo field => AppendField(builder, field),
            PropertyInfo property => AppendProperty(builder, property),
            EventInfo @event => AppendEvent(builder, @event),
            ConstructorInfo ctor => AppendConstructor(builder, ctor),
            MethodInfo method => AppendMethodInfo(builder, method),
            Type type => AppendType(builder, type),
            _ => throw new NotImplementedException(),
        };
    }
}