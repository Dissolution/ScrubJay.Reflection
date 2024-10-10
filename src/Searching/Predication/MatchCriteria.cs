namespace ScrubJay.Reflection.Searching.Predication;

public abstract record class MatchCriteria<T> : ICriteria<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static bool Matches<V>(ICriteria<V>? criteria, V value)
    {
        if (criteria is null)
            return true;
        return criteria.Matches(value);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static bool Matches<V>(ICriteria<V>? criteria, Func<V> getValue)
    {
        if (criteria is null)
            return true;
        return criteria.Matches(getValue());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static bool HasAnyFlags<E>(E? enumFlags, E flag)
        where E : struct, Enum
    {
        if (!enumFlags.HasValue)
            return true;
        return enumFlags.GetValueOrDefault().HasAnyFlags(flag);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static bool HasAnyFlags<E>(E? enumFlags, Func<E> getFlag)
        where E : struct, Enum
    {
        if (!enumFlags.HasValue)
            return true;
        return enumFlags.GetValueOrDefault().HasAnyFlags(getFlag());
    }
    
    public MatchCriteria() { }
    
    public abstract bool Matches(T value);
}