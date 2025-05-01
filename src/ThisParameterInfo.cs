using ScrubJay.Reflection.Validation;

namespace ScrubJay.Reflection;

/// <summary>
/// An instance of the <c>this</c> <see cref="ParameterInfo"/> for an instance <see cref="MethodBase"/>
/// </summary>
[PublicAPI]
public sealed class ThisParameterInfo : ParameterInfo
{
    public override bool HasDefaultValue => false;
    
    public ThisParameterInfo(MethodBase method)
    {
        MemberAssert.IsInstance(method);
        
        this.MemberImpl = method;
        this.ClassImpl = method.DeclaringType.ThrowIfNull();
        this.NameImpl = "this";
        this.PositionImpl = 0;
    }

    public override object[] GetCustomAttributes(bool inherit) => [];
    
    public override object[] GetCustomAttributes(Type? attributeType, bool inherit) => [];
    
    public override IList<CustomAttributeData> GetCustomAttributesData() => [];
}