namespace ScrubJay.Reflection.Utilities;

[PublicAPI]
public sealed class ThisParameter : ParameterInfo
{
    public ThisParameter(MethodBase method) : base()
    {
        this.MemberImpl = method;
        this.ClassImpl = method.DeclaringType;
        this.NameImpl = "this";
        this.PositionImpl = 0;
    }
}