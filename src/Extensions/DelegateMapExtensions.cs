using System.Collections.Concurrent;
using System.Diagnostics;
using ScrubJay.Collections;

namespace ScrubJay.Reflection.Extensions;

public static class DelegateMapExtensions
{
    public static bool TryGet<T, TDelegate>(this ConcurrentTypeMap<Delegate> typeDelegateMap,
        [NotNullWhen(true), MaybeNullWhen(false)]
        out TDelegate? tDelegate)
        where TDelegate : Delegate
    {
        if (typeDelegateMap.TryGetValue<T>(out var @delegate))
        {
            if (@delegate is TDelegate)
            {
                tDelegate = (TDelegate)@delegate;
                return true;
            }
        }
        tDelegate = default;
        return false;
    }

    public static TDelegate GetOrAdd<T, TDelegate>(this ConcurrentTypeMap<Delegate> typeDelegateMap,
        Func<Type, TDelegate> createDelegate)
        where TDelegate : Delegate
    {
        var del = typeDelegateMap.GetOrAdd<T>(createDelegate);
        if (del is TDelegate)
            return (TDelegate)del;
        Debug.Fail("Should not get here");
        return createDelegate(typeof(T));
    }
    
    public static TDelegate GetOrAdd<TDelegate>(this ConcurrentTypeMap<Delegate> typeDelegateMap,
        Type key,
        Func<Type, TDelegate> createDelegate)
        where TDelegate : Delegate
    {
        var del = typeDelegateMap.GetOrAdd(key, createDelegate);
        if (del is TDelegate)
            return (TDelegate)del;
        Debug.Fail("Should not get here");
        return createDelegate(key);
    }
    
    
    
    
    public static bool TryGet<TKey, TDelegate>(
        this ConcurrentDictionary<TKey, Delegate> delegateDict,
        TKey key,
        [NotNullWhen(true), MaybeNullWhen(false)]
        out TDelegate? tDelegate)
        where TKey : notnull
        where TDelegate : Delegate
    {
        if (delegateDict.TryGetValue(key, out var @delegate))
        {
            if (@delegate is TDelegate)
            {
                tDelegate = (TDelegate)@delegate;
                return true;
            }
        }
        tDelegate = default;
        return false;
    }

    public static TDelegate GetOrAdd<TKey, TDelegate>(
        this ConcurrentDictionary<TKey, Delegate> delegateDict,
        TKey key,
        Func<TKey, TDelegate> createDelegate)
    where TKey : notnull
        where TDelegate : Delegate
    {
        var del = delegateDict.GetOrAdd(key, createDelegate);
        if (del is TDelegate)
            return (TDelegate)del;
        Debug.Fail("Should not get here");
        return createDelegate(key);
    }
}