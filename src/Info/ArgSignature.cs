namespace ScrubJay.Reflection.Info;

public abstract record class ArgSignature
{
    public IReadOnlyList<Attribute> Attributes { get; init; } = Array.Empty<Attribute>();
    
    public ReferenceType ReferenceType { get; init; } = ReferenceType.Default;
    
    public required Type Type { get; init; }
    
    public virtual ParameterAttributes GetParameterAttributes()
    {
        ParameterAttributes attrs = default;
        if (ReferenceType.HasFlags(ReferenceType.In))
            attrs |= ParameterAttributes.In;
        if (ReferenceType.HasFlags(ReferenceType.Out))
            attrs |= ParameterAttributes.Out;
        return attrs;
    }
}