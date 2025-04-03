using System.Collections.Concurrent;
using ScrubJay.Reflection.Utilities;

namespace ScrubJay.Reflection.Collections;

internal sealed class TypeArrayEqualityComparer : IEqualityComparer<Type[]>, IHasDefault<TypeArrayEqualityComparer>
{
    public static TypeArrayEqualityComparer Default { get; } = new();

    public bool Equals(Type[]? x, Type[]? y)
    {
        return Sequence.Equal(x, y);
    }
    public int GetHashCode(Type[]? types)
    {
        return Hasher.HashMany<Type>(types);
    }
}


public class DelegateMap
{
    private readonly ConcurrentDictionary<Type[], Delegate> _map = new(TypeArrayEqualityComparer.Default);

    internal static Type[] GetKey<D>()
        where D : Delegate
    {
        var delegateType = typeof(D);
        var genericTypes = delegateType.GetGenericArguments();
        var invoke = DelegateHelper.GetInvokeMethod(delegateType);
        var (_, ret) = invoke.ReturnParameter;
        var pTypes = invoke.GetParameterTypes();
        var pCount = pTypes.Length;

        int cnt = pCount;
        if (ret != typeof(void))
            cnt++;
        Debug.Assert(cnt == genericTypes.Length);

        Type[] key = new Type[pCount + 1];
        Sequence.CopyTo(pTypes, key);
        key[pCount] = ret;

        return key;
    }
    
    public bool Contains<D>()
        where D : Delegate
    {
        return _map.ContainsKey(GetKey<D>());
    }

    public bool TryGet<D>(
        [NotNullWhen(true), MaybeNullWhen(false)] 
        out D @delegate)
        where D : Delegate
    {
        if (_map.TryGetValue(GetKey<D>(), out var del) &&
            del.Is<D>(out @delegate))
            return true;
        
        @delegate = null;
        return false;
    }

    public D GetOrAdd<D>(D addDelegate)
        where D : Delegate
    {
        var del = _map.GetOrAdd(GetKey<D>(), addDelegate);
        return (D)del;
    }
    
    public D GetOrAdd<D>(Func<D> createDelegate)
        where D : Delegate
    {
        var del = _map.GetOrAdd(GetKey<D>(), key => createDelegate());
        return (D)del;
    }
}