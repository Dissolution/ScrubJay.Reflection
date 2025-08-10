using System.Collections.Concurrent;
using ScrubJay.Reflection.Comparison;

namespace ScrubJay.Reflection.Collections;

public class DelegateMap : IEnumerable<Delegate>
{
    private readonly ConcurrentDictionary<Type[], Delegate> _map = new(TypeArrayEqualityComparer.Default);

    public static Type[] GetKey<D>()
        where D : Delegate
    {
        var delegateType = typeof(D);
        //var genericTypes = delegateType.GetGenericArguments();
        var invoke = DelegateHelper.GetInvokeMethod<D>();
        var (_, ret) = invoke.ReturnParameter;
        var pTypes = invoke.GetParameterTypes();
        var pCount = pTypes.Length;

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

    public bool TryAdd<D>(D del)
        where D : Delegate
    {
        return _map.TryAdd(GetKey<D>(), del);
    }

    public void Add<D>(D del)
        where D : Delegate
        => _map.AddOrUpdate(GetKey<D>(), del, (_, _) => del);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<Delegate> GetEnumerator() => _map.Values.GetEnumerator();
}