using System.Collections;
using ScrubJay.Sigil.Utilities;

namespace ScrubJay.Sigil.Impl;

internal class LinqStack<T> : IEnumerable<T>
    where T : class
{
    public new int Count { get { return _inner.Count; } }

    private LinqStack<T> _inner;

    private LinqStack(LinqStack<T> i)
    {
        _inner = i;
    }

    public LinqStack() : this(new LinqStack<T>()) { }
    public LinqStack(IEnumerable<T> e) : this(new LinqStack<T>(e)) { }
    public LinqStack(int n) : this(new LinqStack<T>(n)) { }

    protected IEnumerable<T> InnerEnumerable()
    {
        return _inner;
    }

    public T Pop()
    {
        return _inner.Pop();
    }

    public void Push(T t)
    {
        _inner.Push(t);
    }

    public T Peek()
    {
        return _inner.Peek();
    }

    public void Clear()
    {
        _inner.Clear();
    }

    private static List<TypeOnStack> _peekWildcard = new List<TypeOnStack>(new[] { TypeOnStack.Get<WildcardType>() });
    public List<TypeOnStack>[] Peek(bool baseless, int n)
    {
        var stack = this;

        if (stack.Count < n && !baseless) return null;

        var ret = new List<TypeOnStack>[n];

        int i;
        for (i = 0; i < n && i < stack.Count; i++)
        {
            ret[i] = stack.ElementAt(i) as List<TypeOnStack>;
        }

        while (i < n)
        {
            ret[i] = _peekWildcard;
            i++;
        }

        return ret;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();
}
