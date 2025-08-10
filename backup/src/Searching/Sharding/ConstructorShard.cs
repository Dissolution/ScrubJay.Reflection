
using ScrubJay.Reflection.Collections;

namespace ScrubJay.Reflection.Searching.Sharding;

public abstract class ConstructorShard<S> : MethodBaseShard<S, ConstructorInfo>
    where S : ConstructorShard<S>
{
    protected ConstructorShard(DLCL<ConstructorInfo> ctors, List<string> filters) 
        : base(ctors, filters)
    {
    }
}

[PublicAPI]
public sealed class ConstructorShard : ConstructorShard<ConstructorShard>
{
    internal ConstructorShard(DLCL<ConstructorInfo> ctors, List<string> filters) : base(ctors, filters)
    {
    }
}