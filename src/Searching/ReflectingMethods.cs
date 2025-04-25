namespace ScrubJay.Reflection.Searching;

public sealed class ReflectingMethods : ReflectingMethodInfos<ReflectingMethods>
{
    public ReflectingMethods(IEnumerable<MethodInfo> methods)
        : base(methods)
    {
    }
}

public abstract class ReflectingMethodInfos<B> : ReflectingMethodBases<B, MethodInfo>
    where B : ReflectingMethodInfos<B>
{
    protected ReflectingMethodInfos(IEnumerable<MethodInfo> methods) 
        : base(methods)
    {
    }

//    public B Returning(Type type)
//    {
//        return Only(type, static (method,t) => method.ReturnType == t);
//    }
//
//    public B Returning(Type type, TypeMatch match)
//    {
//        return Only(type, match, static (method,t,m) => method.ReturnType.Matches(t,m));
//    }
//
//    public B Returning<T>() => Returning(typeof(T));
//
//    public B Returning<T>(TypeMatch match) => Returning(typeof(T), match);
}