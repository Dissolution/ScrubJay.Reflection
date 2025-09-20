namespace ScrubJay.Reflection;

[PublicAPI]
public class OverridableParameterInfo : ParameterInfo
{
    private IList<CustomAttributeData> _customAttributeData = [];
    
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

    [AllowNull]
    public new Type ParameterType
    {
        get => base.ClassImpl ?? typeof(void);
        set => base.ClassImpl = value ?? typeof(void);
    }

    public new IEnumerable<CustomAttributeData> CustomAttributes
    {
        get => base.CustomAttributes;
        set => _customAttributeData = value.ToList();
    }

    public Option<object?> Default { get; set; } = Option<object?>.None;

    public sealed override bool HasDefaultValue => this.Default.IsSome();

    public sealed override object? DefaultValue => Default.SomeOr(DBNull.Value);

    public sealed override object? RawDefaultValue => Default.SomeOr(DBNull.Value);
    
    public OverridableParameterInfo()
    {
        
    }

    public OverridableParameterInfo(ParameterInfo parameter)
    {
        base.AttrsImpl = parameter.Attributes;
        base.ClassImpl = parameter.ParameterType;
        base.PositionImpl = parameter.Position;
        base.NameImpl = parameter.Name;
        this.Default = parameter.Default;
        this.CustomAttributes = parameter.CustomAttributes;
    }

    public sealed override object[] GetCustomAttributes(bool inherit)
    {
        var attributes = Attribute.GetCustomAttributes(this, inherit);
        return attributes.ConvertAll(attr => (object)attr);
    }

    public sealed override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
        var attributes = Attribute.GetCustomAttributes(this, attributeType, inherit);
        return attributes.ConvertAll(attr => (object)attr);
    }

    public sealed override IList<CustomAttributeData> GetCustomAttributesData()
    {
        return _customAttributeData;
    }
}