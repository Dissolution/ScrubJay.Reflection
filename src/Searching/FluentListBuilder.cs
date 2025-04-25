namespace ScrubJay.Reflection.Searching;



[PublicAPI]
public abstract class FluentListBuilder<B, T> : BuilderBase<B>, IEnumerable<T>
    where B : FluentListBuilder<B, T>
{
    protected readonly List<T> _values;
    protected readonly List<string> _restrictions = [];

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

    public B Only(Func<T, bool> predicate,
        [CallerMemberName] string? callerName = null)
    {
        _values.RemoveAll(member => !predicate(member));
        Debug.Assert(callerName is not null);
        _restrictions.Add(callerName!);
        return _builder;
    }

    public B Only<S1>(S1 state1, Func<T, S1, bool> predicate,
        [CallerMemberName] string? callerName = null)
    {
        _values.RemoveAll(member => !predicate(member, state1));
        Debug.Assert(callerName is not null);
        string restriction =TextBuilder.New
            .Append(callerName)
            .Append('(')
            .Render(state1)
            .Append(')')
            .ToStringAndDispose();
        _restrictions.Add(restriction);
        return _builder;
    }
    
    public B Only<S1, S2>(S1 state1, S2 state2, Func<T, S1, S2, bool> predicate,
        [CallerMemberName] string? callerName = null)
    {
        _values.RemoveAll(member => !predicate(member, state1, state2));
        Debug.Assert(callerName is not null);
        string restriction =TextBuilder.New
            .Append(callerName)
            .Append('(')
            .Render(state1)
            .Append(", ")
            .Render(state2)
            .Append(')')
            .ToStringAndDispose();
        _restrictions.Add(restriction);
        return _builder;
    }

    public B Only<S1, S2, S3>(S1 state1, S2 state2, S3 state3, Func<T, S1, S2, S3, bool> predicate,
        [CallerMemberName] string? callerName = null)
    {
        _values.RemoveAll(member => !predicate(member, state1, state2, state3));
        Debug.Assert(callerName is not null);
        string restriction =TextBuilder.New
            .Append(callerName)
            .Append('(')
            .Render(state1)
            .Append(", ")
            .Render(state2)
            .Append(", ")
            .Render(state3)
            .Append(')')
            .ToStringAndDispose();
        _restrictions.Add(restriction);
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

    public T OneOrThrow(string? message = null)
    {
        if (_values.Count == 1)
            return _values[0];

        var error = TextBuilder.New
            .LineDelimitAppend(_restrictions)
            .IfNotNull(message, static (tb,msg) => tb.NewLine().Append($"Info: {msg}"))
            .ToStringAndDispose();
        throw new InvalidOperationException(error);
    }

    public override string ToString() => TextBuilder.New
        .Append('[')
        .DelimitAppend(", ", _values)
        .Append(']')
        .ToStringAndDispose();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    public List<T>.Enumerator GetEnumerator() => _values.GetEnumerator();
}