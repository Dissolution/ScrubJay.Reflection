namespace ScrubJay.Reflection.Searching;

public sealed class MirrorConstructors : MirrorConstructorInfosBuilder<MirrorConstructors>
{
    internal MirrorConstructors(Type reflectedType, IEnumerable<ConstructorInfo> members) 
        : base(reflectedType, members)
    {
    }
}

public abstract class MirrorConstructorInfosBuilder<B> : MirrorMethodBaseBuilder<B, ConstructorInfo>
    where B : MirrorConstructorInfosBuilder<B>
{
    protected MirrorConstructorInfosBuilder(Type reflectedType, IEnumerable<ConstructorInfo> members) 
        : base(reflectedType, members)
    {
    }
}