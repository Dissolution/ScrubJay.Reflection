namespace ScrubJay.Reflection.Collections;




public sealed class DLCLNode<T> 
{
    internal DLCL<T>? _dlclParent;
    internal DLCLNode<T>? _next;
    internal DLCLNode<T>? _prev;
    internal T _value;

    public DLCLNode(T value)
    {
        _value = value;
    }

    internal DLCLNode(DLCL<T> list, T value)
    {
        this._dlclParent = list;
        _value = value;
    }

    public DLCL<T>? List => _dlclParent;

    public DLCLNode<T>? Next => _next == null || _next == _dlclParent!._head ? null : _next;

    public DLCLNode<T>? Previous => _prev == null || this == _dlclParent!._head ? null : _prev;

    public T Value
    {
        get => _value;
        set => _value = value;
    }
    
    public ref T ValueRef => ref _value;

    // terminate at end
    public DLCLNode<T>? DeleteAndNext()
    {
        var next = _next;
        _dlclParent?.InternalRemoveNode(this);
        return next;
    }

    public DLCLNode<T>? AddNext(T value)
    {
        var node = new DLCLNode<T>(_dlclParent!, value);
        node._prev = this;
        node._next = _next;
        _next!._prev = node;
        _next = node;
        return node;
    }

    public DLCLNode<T>? AddPrevious(T value)
    {
        var node = new DLCLNode<T>(_dlclParent!, value);
        node._next = this;
        node._prev = _prev;
        _prev!._next = node;
        _prev = node;
        return node;
    }

    internal void Invalidate()
    {
        _dlclParent = null;
        _next = null;
        _prev = null;
    }
}