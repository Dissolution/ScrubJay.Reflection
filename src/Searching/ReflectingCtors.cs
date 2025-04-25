namespace ScrubJay.Reflection.Searching;

public sealed class ReflectingCtors : ReflectingConstructorInfos<ReflectingCtors>
{
    public ReflectingCtors(IEnumerable<ConstructorInfo> constructors) 
        : base(constructors)
    {
    }
}

public abstract class ReflectingConstructorInfos<B> : ReflectingMethodBases<B, ConstructorInfo>
    where B : ReflectingConstructorInfos<B>
{
    protected ReflectingConstructorInfos(IEnumerable<ConstructorInfo> constructors) 
        : base(constructors)
    {
    }
}