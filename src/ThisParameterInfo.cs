namespace ScrubJay.Reflection;

public sealed class ThisParameterInfo : ParameterInfo
{
    public override bool HasDefaultValue => false;

    public ThisParameterInfo(MethodBase method)
    {
        Debug.Assert(!method.IsStatic);
        this.MemberImpl = method;
        this.ClassImpl = method.DeclaringType.ThrowIfNull();
        this.NameImpl = "this";
        this.PositionImpl = 0;
    }

    public override object[] GetCustomAttributes(bool inherit) => [];
    public override object[] GetCustomAttributes(Type? attributeType, bool inherit) => [];
    public override IList<CustomAttributeData> GetCustomAttributesData() => [];
}