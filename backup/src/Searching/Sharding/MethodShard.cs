
using ScrubJay.Reflection.Collections;

namespace ScrubJay.Reflection.Searching.Sharding;

public abstract class MethodShard<S> : MethodBaseShard<S, MethodInfo>
    where S : MethodShard<S>
{
    protected MethodShard(DLCL<MethodInfo> methods, List<string> filters) 
        : base(methods, filters)
    {
    }

    public S Async()
    {
        return AddFilter(static method => method.IsAsync(), "is async");
    }
    
    public S NonAsync()
    {
        return AddFilter(static method => !method.IsAsync(), "is non-async");
    }
}

[PublicAPI]
public sealed class MethodShard : MethodShard<MethodShard>
{
    internal MethodShard(DLCL<MethodInfo> methods, List<string> filters) : base(methods, filters)
    {
    }
}