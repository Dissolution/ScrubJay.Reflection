using System.Reflection;

namespace ScrubJay.Reflection.OLD.OLD.Scratch;

public record class RuntimeParameter
{
    public int Position { get; set; } = -1; // invalid
    public string? Name { get; set; } = null;
    public Type? ValueType { get; set; } = null;
    public ReferenceType ReferenceType { get; set; } = ReferenceType.Default;
    public bool IsOptional { get; set; } = false;
    public Option<object?> DefaultValue { get; set; } = None();

    public virtual ParameterAttributes ParameterAttributes
    {
        get
        {
            ParameterAttributes attr = default;
            if (IsOptional)
                attr.AddFlag(ParameterAttributes.Optional);
            if (DefaultValue.IsSome)
                attr.AddFlag(ParameterAttributes.HasDefault);
            if (ReferenceType.HasFlags(ReferenceType.In))
                attr.AddFlag(ParameterAttributes.In);
            if (ReferenceType.HasFlags(ReferenceType.Out))
                attr.AddFlag(ParameterAttributes.Out);
            return attr;
        }
    }

    public virtual Type? CompositeType
    {
        get
        {
            if (ReferenceType == ReferenceType.Default)
            {
                return ValueType;
            }

            if (ReferenceType.HasAnyFlags(ReferenceType.Ref, ReferenceType.In, ReferenceType.Out))
            {
                return ValueType?.MakeByRefType();
            }

            return ValueType;
        }
    }
}