namespace ScrubJay.Reflection.Info;

public record class ReturnSignature : ArgSignature
{
    public static ReturnSignature Void { get; } = Create(typeof(void));

    
    public static ReturnSignature Create(Type type)
    {
        return new ReturnSignature
        {
            Attributes = Attribute.GetCustomAttributes(type),
            ReferenceType = type.IsReference(),
            Type = type,
        };
    }
    
    public static ReturnSignature Create(ParameterInfo parameter)
    {
        return new ReturnSignature
        {
            Attributes = Attribute.GetCustomAttributes(parameter),
            ReferenceType = parameter.RefKind(),
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