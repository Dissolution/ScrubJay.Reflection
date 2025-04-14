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
        return Only(type, static (method,t) => method.ReturnType == t);
    }

    public B Returning(Type type, TypeMatch match)
    {
        return Only(type, match, static (method,t,m) => method.ReturnType.Matches(t,m));
    }

    public B Returning<T>() => Returning(typeof(T));

    public B Returning<T>(TypeMatch match) => Returning(typeof(T), match);
}