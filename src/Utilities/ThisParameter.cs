using ScrubJay.Reflection.Exceptions;

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

[PublicAPI]
public sealed class ReturnParameter : ParameterInfo
{
    public ReturnParameter(MethodBase method) : base()
    {
        this.MemberImpl = method;
        this.PositionImpl = -1;
        this.NameImpl = "return";

        if (method is MethodInfo methodInfo)
        {
            this.ClassImpl = methodInfo.ReturnType;
        }
        else if (method is ConstructorInfo constructorInfo)
        {
            this.ClassImpl = constructorInfo.DeclaringType;
        }
        else
        {
            throw new MemberException(method, $"Invalid MethodBase type: {method.GetType():@}");
        }
    }
}