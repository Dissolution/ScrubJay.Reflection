namespace ScrubJay.Reflection.Naming;

/// <summary>
/// Helper Utility for getting the names of <see cref="ParameterInfo"/> and <see cref="MemberInfo"/>
/// </summary>
[PublicAPI]
public static class MemberNames
{
    private static TBuilder AppendNullability<TBuilder>(this TBuilder text, NullabilityInfo? nullabilityInfo)
        where TBuilder : TextBuilderBase<TBuilder>
    {
        if (nullabilityInfo is null)
            return text;

        NullabilityState readState = nullabilityInfo.ReadState;
        var writeState = nullabilityInfo.WriteState;

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

    public static TBuilder AppendParameter<TBuilder>(this TBuilder text, ParameterInfo parameter)
        where TBuilder : TextBuilderBase<TBuilder>
    {
        var refKind = parameter.ReferenceKind(out var parameterType);
        return text
            .Append(refKind.AsString())
            .AppendType(parameterType)
            .AppendNullability(parameter.NullabilityInfo())
            .If(Validate.IsNotEmpty(parameter.Name),
                static (tb, name) => tb.Append(' ').Append(name))
            .If(parameter.DefaultOption(),
                static (tb, defaultValue) => tb.Append(" = ").Append(defaultValue));
    }

    public static TBuilder AppendField<TBuilder>(this TBuilder text, FieldInfo? field)
        where TBuilder : TextBuilderBase<TBuilder>
    {
        if (field is null)
            return text;
        return text
            .AppendType(field.FieldType)
            .AppendNullability(field.NullabilityInfo())
            .Append(' ')
            .If(field.IsStatic,
                tb => tb.AppendType(field.OwnerType()),
                static tb => tb.Append("this"))
            .Append('.')
            .Append(field.Name);
    }

    public static TBuilder AppendProperty<TBuilder>(this TBuilder text, PropertyInfo? property)
        where TBuilder : TextBuilderBase<TBuilder>
    {
        if (property is null)
            return text;
        return text
            .AppendType(property.PropertyType)
            .AppendNullability(property.NullabilityInfo())
            .Append(' ')
            .If(property.IsStatic(),
                tb => tb.AppendType(property.OwnerType()),
                static tb => tb.Append("this"))
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
            .AppendType(@event.EventHandlerType)
            .AppendNullability(@event.NullabilityInfo())
            .Append(' ')
            .If(@event.IsStatic(),
                tb => tb.AppendType(@event.OwnerType()),
                static tb => tb.Append("this"))
            .Append('.')
            .Append(@event.Name);
    }

    public static TBuilder AppendConstructor<TBuilder>(this TBuilder text, ConstructorInfo? ctor)
        where TBuilder : TextBuilderBase<TBuilder>
    {
        if (ctor is null)
            return text;
        return text
            .AppendType(ctor.OwnerType())
            .Append(ctor.Name)
            .If(Validate.IsNotEmpty(ctor.GetGenericArguments()),
                static (tb, genericTypes) => tb
                    .Append('<')
                    .Delimit(", ", genericTypes, static (t,a) => t.AppendType(a))
                    .Append('>'))
            .Append('(')
            .Delimit(", ", ctor.GetParameters(), static (tb, param) => AppendParameter(tb, param))
            .Append(')');
    }

    public static TBuilder AppendMethod<TBuilder>(this TBuilder text, MethodInfo? method)
        where TBuilder : TextBuilderBase<TBuilder>
    {
        if (method is null)
            return text;
        return text
            .AppendParameter(method.ReturnParameter)
            .Append(' ')
            .If(method.IsStatic,
                tb => tb.AppendType(method.OwnerType()),
                tb => tb.Append("this"))
            .Append('.')
            .Append(method.Name)
            .If(method.IsGenericMethod,
                tb => tb.Append('<').Delimit(", ", method.GetGenericArguments(), static (t,a) => t.AppendType(a)).Append('>'))
            .Append('(')
            .Delimit(", ", method.GetParameters(), static (tb, param) => AppendParameter(tb, param))
            .Append(')');
    }

    public static TBuilder AppendMember<TBuilder>(this TBuilder text, MemberInfo? member)
        where TBuilder : TextBuilderBase<TBuilder>
    {
        return member switch
        {
            null => text.Append("null"),
            FieldInfo field => AppendField(text, field),
            PropertyInfo property => AppendProperty(text, property),
            EventInfo @event => AppendEvent(text, @event),
            ConstructorInfo ctor => AppendConstructor(text, ctor),
            MethodInfo method => AppendMethod(text, method),
            Type type => text.AppendType(type),
            _ => throw new NotImplementedException(),
        };
    }

    public static string NameOf(this ParameterInfo parameter)
    {
        return TextBuilder.New.Invoke(tb => AppendParameter(tb, parameter)).ToStringAndDispose();
    }

    public static string NameOf(this MemberInfo member)
    {
        return TextBuilder.New.Invoke(tb => AppendMember(tb, member)).ToStringAndDispose();
    }
}
