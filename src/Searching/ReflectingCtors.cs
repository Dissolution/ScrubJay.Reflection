namespace ScrubJay.Reflection.Searching;

[PublicAPI]
public sealed class ReflectingCtors : ReflectingConstructorInfos<ReflectingCtors>
{
    public ReflectingCtors(IEnumerable<ConstructorInfo> constructors) 
        : base(constructors)
    {
    }
}

[PublicAPI]
public abstract class ReflectingConstructorInfos<B> : ReflectingMethodBases<B, ConstructorInfo>
    where B : ReflectingConstructorInfos<B>
{
    protected ReflectingConstructorInfos(IEnumerable<ConstructorInfo> constructors) 
        : base(constructors)
    {
    }
}