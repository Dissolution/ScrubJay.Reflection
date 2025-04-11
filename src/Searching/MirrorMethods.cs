namespace ScrubJay.Reflection.Searching;

public sealed class MirrorMethods : MirrorMethodsBuilder<MirrorMethods>
{
    internal MirrorMethods(Type reflectedType, IEnumerable<MethodInfo> members)
        : base(reflectedType, members)
    {
    }
}

public abstract class MirrorMethodsBuilder<B> : MirrorMethodBaseBuilder<B, MethodInfo>
    where B : MirrorMethodsBuilder<B>
{
    protected MirrorMethodsBuilder(Type reflectedType, IEnumerable<MethodInfo> members) : base(reflectedType, members)
    {
    }

    public B Returning(Type type)
    {
        return Where(method => method.ReturnType == type);
    }

    public B Returning(Type type, TypeMatch match)
    {
        return Where(method => method.ReturnType.Matches(type, match));
    }

    public B Returning<T>() => Returning(typeof(T));

    public B Returning<T>(TypeMatch match) => Returning(typeof(T), match);
}