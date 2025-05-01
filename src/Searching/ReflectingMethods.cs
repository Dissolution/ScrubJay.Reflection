namespace ScrubJay.Reflection.Searching;

[PublicAPI]
public sealed class ReflectingMethods : ReflectingMethodInfos<ReflectingMethods>
{
    public ReflectingMethods(IEnumerable<MethodInfo> methods)
        : base(methods)
    {
    }
}

[PublicAPI]
public abstract class ReflectingMethodInfos<B> : ReflectingMethodBases<B, MethodInfo>
    where B : ReflectingMethodInfos<B>
{
    protected ReflectingMethodInfos(IEnumerable<MethodInfo> methods)
        : base(methods)
    {
    }
}