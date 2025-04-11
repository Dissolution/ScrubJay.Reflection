using ScrubJay.Fluent;

namespace ScrubJay.Reflection.Searching;

[PublicAPI]
public abstract class FluentListBuilder<B, T> : FluentBuilder<B>
    where B : FluentListBuilder<B, T>
{
    protected readonly List<T> _values;

    protected FluentListBuilder(IEnumerable<T>? values = null)
    {
        if (values is null)
        {
            _values = [];
        }
        else
        {
            _values = new List<T>(values);
        }
    }
    
    public B Where(Func<T, bool> predicate)
    {
        _values.RemoveAll(member => !predicate(member));
        return _builder;
    }

    public IEnumerable<N> Select<N>(Func<T, N> selector) => _values.Select(selector);

    public List<T> AsList() => _values;

    public Option<T> First()
    {
        if (_values.Count > 0)
            return Some(_values[0]);
        return None();
    }
    
    public Option<T> One()
    {
        if (_values.Count == 1)
            return Some(_values[0]);
        return None();
    }

    public T OneOrThrow(string? message = null) => One().SomeOrThrow(message);
    
    public override string ToString() => TextBuilder.New
        .Append('[')
        .DelimitAppend(", ", _values)
        .Append(']')
        .ToStringAndDispose();
}