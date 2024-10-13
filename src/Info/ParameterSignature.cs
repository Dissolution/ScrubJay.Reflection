namespace ScrubJay.Reflection.Info;

public record class ParameterSignature : ArgSignature
{
    public static ParameterSignature Create(Type type)
    {
        return new ParameterSignature
        {
            Attributes = Attribute.GetCustomAttributes(type),
            ReferenceType = type.IsReference(),
            Type = type,
        };
    }
    
    public static ParameterSignature Create(ParameterInfo parameter)
    {
        return new ParameterSignature
        {
            Attributes = Attribute.GetCustomAttributes(parameter),
            Name = parameter.Name,
            ReferenceType = parameter.RefKind(),
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