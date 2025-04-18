namespace ScrubJay.Reflection.MosDef;

public sealed class ReturnParameterInfo : ParameterInfo
{
    public override ParameterAttributes Attributes { get; } = ParameterAttributes.Retval;

    public override bool HasDefaultValue => false;

    public ReturnParameterInfo(MethodBase method, Type returnType)
    {
        this.MemberImpl = method;
        this.ClassImpl = returnType;
        this.NameImpl = "return";
        this.PositionImpl = -1;
    }

    public override object[] GetCustomAttributes(bool inherit) => [];
    public override object[] GetCustomAttributes(Type? attributeType, bool inherit) => [];
    public override IList<CustomAttributeData> GetCustomAttributesData() => [];
}