using ScrubJay.Fluent;

namespace ScrubJay.Reflection.Searching;

public abstract class FilterBuilder<B, T> : FluentBuilder<B>
    where B : FilterBuilder<B, T>
{
    protected readonly List<T> _values;

    protected FilterBuilder(IEnumerable<T> values)
    {
        _values = values.ToList();
    }

    protected void Filter(Func<T, bool> memberPredicate) 
        => _values.RemoveAll(item => !memberPredicate(item));

    protected IEnumerable<N> OfType<N>()
        where N : MemberInfo
        => _values.OfType<N>();

    protected IEnumerable<N> Select<N>(Func<T, N> selector)
        where N : MemberInfo
        => _values.Select(selector);
    
    
    public B Where(Func<T, bool> memberPredicate)
    {
        Filter(memberPredicate);
        return _builder;
    }

    public Option<T> First()
    {
        if (_values.Count > 0)
            return Some(_values[0]);
        return None<T>();
    }

    public Option<T> One()
    {
        if (_values.Count == 1)
            return Some(_values[0]);
        return None<T>();
    }
    
    public T[] ToArray() => _values.ToArray();
    
    public List<T> ToList() => _values.ToList();
    
    public IEnumerable<T> Enumerate() => _values;
}