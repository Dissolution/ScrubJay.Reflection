namespace ScrubJay.Reflection;

public sealed class SigParameterInfo : ParameterInfo
{
    public new ParameterAttributes Attributes
    {
        get => base.AttrsImpl;
        set => base.AttrsImpl = value;
    }

    public new string? Name
    {
        get => base.NameImpl;
        set => base.NameImpl = value;
    }

    public new int Position
    {
        get => base.PositionImpl;
        set => base.PositionImpl = value;
    }

    [NotNull, AllowNull]
    public new Type ParameterType
    {
        get => base.ClassImpl ?? typeof(void);
        set => base.ClassImpl = value;
    }

    public Option<object?> Default { get; set; }

    public override bool HasDefaultValue => Default.IsSome();

    public override object? DefaultValue => Default.SomeOr(DBNull.Value);

    public override object? RawDefaultValue => Default.SomeOr(DBNull.Value);
    
    public SigParameterInfo()
    {
        
    }

    public SigParameterInfo(ParameterInfo parameter)
    {
        base.AttrsImpl = parameter.Attributes;
        base.ClassImpl = parameter.ParameterType;
        base.PositionImpl = parameter.Position;
        base.NameImpl = parameter.Name;
        this.Default = parameter.Default();
        this.CustomAttributes = parameter.CustomAttributes;
    }

    // public override object[] GetCustomAttributes(bool inherit) => base.GetCustomAttributes(inherit);
    // public override object[] GetCustomAttributes(Type attributeType, bool inherit) => base.GetCustomAttributes(attributeType, inherit);
    // public override IList<CustomAttributeData> GetCustomAttributesData() => base.GetCustomAttributesData();
    // public override bool IsDefined(Type attributeType, bool inherit) => base.IsDefined(attributeType, inherit);
    public override IEnumerable<CustomAttributeData> CustomAttributes { get; }
}