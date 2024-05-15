using static ScrubJay.GlobalHelper;

namespace ScrubJay.Reflection.Info;

public abstract record class ArgSignature
{
    public IReadOnlyList<Attribute> Attributes { get; init; } = Array.Empty<Attribute>();
    
    public RefKind RefKind { get; init; } = RefKind.None;
    
    public required Type Type { get; init; }
    
    public virtual ParameterAttributes GetParameterAttributes()
    {
        ParameterAttributes attrs = default;
        if (RefKind.HasFlags(RefKind.In))
            attrs |= ParameterAttributes.In;
        if (RefKind.HasFlags(RefKind.Out))
            attrs |= ParameterAttributes.Out;
        return attrs;
    }
}

public record class ParameterSignature : ArgSignature
{
    public static ParameterSignature Create(Type type)
    {
        return new ParameterSignature
        {
            Attributes = Attribute.GetCustomAttributes(type),
            RefKind = type.RefKind(),
            Type = type,
        };
    }
    
    public static ParameterSignature Create(ParameterInfo parameter)
    {
        return new ParameterSignature
        {
            Attributes = Attribute.GetCustomAttributes(parameter),
            Name = parameter.Name,
            RefKind = parameter.RefKind(),
            Type = parameter.ParameterType,
            Default = parameter.HasDefaultValue ? Some(parameter.DefaultValue) : None(),
        };
    }

    public string? Name { get; init; } = null;

    public Option<object?> Default { get; init; } = None();

    public override ParameterAttributes GetParameterAttributes()
    {
        var attrs = base.GetParameterAttributes();
        if (this.Default.IsSome())
        {
            attrs.AddFlag(ParameterAttributes.Optional);
            attrs.AddFlag(ParameterAttributes.HasDefault);
        }
        return attrs;
    }
}

public record class ReturnSignature : ArgSignature
{
    public static ReturnSignature Void { get; } = Create(typeof(void));

    
    public static ReturnSignature Create(Type type)
    {
        return new ReturnSignature
        {
            Attributes = Attribute.GetCustomAttributes(type),
            RefKind = type.RefKind(),
            Type = type,
        };
    }
    
    public static ReturnSignature Create(ParameterInfo parameter)
    {
        return new ReturnSignature
        {
            Attributes = Attribute.GetCustomAttributes(parameter),
            RefKind = parameter.RefKind(),
            Type = parameter.ParameterType,
        };
    }
    
    public override ParameterAttributes GetParameterAttributes()
    {
        var attrs = base.GetParameterAttributes();
        attrs.AddFlag(ParameterAttributes.Retval);
        return attrs;
    }
}