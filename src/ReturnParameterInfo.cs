namespace ScrubJay.Reflection;

/// <summary>
/// An instance of the <c>return</c> <see cref="ParameterInfo"/> for a <see cref="MethodBase"/>
/// </summary>
[PublicAPI]
public sealed class ReturnParameterInfo : ParameterInfo
{
    public override bool HasDefaultValue => false;

    public override object? DefaultValue => DBNull.Value;

    public override object? RawDefaultValue => DBNull.Value;

    public ReturnParameterInfo(MethodBase method, Type? returnType = null)
    {
        this.AttrsImpl = ParameterAttributes.Retval;
        this.MemberImpl = method;
        this.ClassImpl = returnType ?? method.ReturnType();
        this.NameImpl = "return";
        this.PositionImpl = -1;
    }
    
    public override object[] GetCustomAttributes(bool inherit) => [];
    
    public override object[] GetCustomAttributes(Type? attributeType, bool inherit) => [];
    
    public override IList<CustomAttributeData> GetCustomAttributesData() => [];
}