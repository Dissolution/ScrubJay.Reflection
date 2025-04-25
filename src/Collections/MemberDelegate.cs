using System.Collections.Concurrent;

namespace ScrubJay.Reflection.Collections;


[PublicAPI]
public sealed record class MemberDelegate<M,D>(M Member, D Delegate)
    where M : MemberInfo
    where D : Delegate;

public sealed class MemberDelegateCache
{
    private readonly ConcurrentDictionary<Type[], object> _cache = new(TypeArrayEqualityComparer.Default);

    public Option<MemberDelegate<M, D>> Get<M, D>()
        where M : MemberInfo
        where D : Delegate
    {
        Type[] key = [typeof(M), typeof(D)];
        if (_cache.TryGetValue(key, out var value))
            return value.Is<MemberDelegate<M, D>>();
        return None();
    }
}