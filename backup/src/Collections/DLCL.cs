
namespace ScrubJay.Reflection.Collections;

/// <summary>
/// This <see cref="DLCL{T}"/> is a doubly-linked circular list
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
[DebuggerDisplay("Count = {Count}")]
public class DLCL<T> : IEnumerable<T>
{
    public static DLCL<T> New(T[]? items)
    {
        var ll = new DLCL<T>();
        if (items is not null)
        {
            foreach (var item in items)
            {
                ll.AddLast(item);
            }
        }

        return ll;
    }
    
    public static DLCL<T> New(IEnumerable<T>? items)
    {
        var ll = new DLCL<T>();
        if (items is not null)
        {
            foreach (var item in items)
            {
                ll.AddLast(item);
            }
        }

        return ll;
    }
    
    internal DLCLNode<T>? _head;
    internal int _count;
    
    public int Count => _count;

    public DLCLNode<T>? First => _head;

    public DLCLNode<T>? Last => _head?._prev;

    public DLCL()
    {
        _head = null;
        _count = 0;
    }
    
    private void InternalInsertNodeBefore(DLCLNode<T> targetNode, DLCLNode<T> newNode)
    {
        newNode._next = targetNode;
        newNode._prev = targetNode._prev;
        targetNode._prev!._next = newNode;
        targetNode._prev = newNode;
        _count++;
    }

    private void InternalInsertFirstNode(DLCLNode<T> newNode)
    {
        Debug.Assert(_head is null && _count == 0);
        newNode._next = newNode;
        newNode._prev = newNode;
        _head = newNode;
        _count = 1;
    }

    internal void InternalRemoveNode(DLCLNode<T> node, bool invalidate = true)
    {
        Debug.Assert(node._dlclParent == this);
        Debug.Assert(_head != null);
        
        // Is this the only node?
        if (node._next == node)
        {
            Debug.Assert(_count == 1 && _head == node);
            _head = null;
        }
        else
        {
            // cut the node out
            node._next!._prev = node._prev;
            node._prev!._next = node._next;
            if (_head == node)
            {
                _head = node._next;
            }
        }
        if (invalidate)
            node.Invalidate();
        _count--;
    }

    internal static void ValidateNewNode(DLCLNode<T> node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (node._dlclParent != null)
        {
            throw new InvalidOperationException();
        }
    }

    internal void ValidateNode(DLCLNode<T> node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (node._dlclParent != this)
        {
            throw new InvalidOperationException();
        }
    }


    

    public DLCLNode<T> AddAfter(DLCLNode<T> node, T value)
    {
        ValidateNode(node);
        DLCLNode<T> result = new DLCLNode<T>(node._dlclParent!, value);
        InternalInsertNodeBefore(node._next!, result);
        return result;
    }

    public void AddAfter(DLCLNode<T> node, DLCLNode<T> newNode)
    {
        ValidateNode(node);
        ValidateNewNode(newNode);
        InternalInsertNodeBefore(node._next!, newNode);
        newNode._dlclParent = this;
    }

    public DLCLNode<T> AddBefore(DLCLNode<T> node, T value)
    {
        ValidateNode(node);
        DLCLNode<T> result = new DLCLNode<T>(node._dlclParent!, value);
        InternalInsertNodeBefore(node, result);
        if (node == _head)
        {
            _head = result;
        }
        return result;
    }

    public void AddBefore(DLCLNode<T> node, DLCLNode<T> newNode)
    {
        ValidateNode(node);
        ValidateNewNode(newNode);
        InternalInsertNodeBefore(node, newNode);
        newNode._dlclParent = this;
        if (node == _head)
        {
            _head = newNode;
        }
    }

    public DLCLNode<T> AddFirst(T value)
    {
        DLCLNode<T> result = new DLCLNode<T>(this, value);
        if (_head == null)
        {
            InternalInsertFirstNode(result);
        }
        else
        {
            InternalInsertNodeBefore(_head, result);
            _head = result;
        }
        return result;
    }

    public void AddFirst(DLCLNode<T> node)
    {
        ValidateNewNode(node);

        if (_head == null)
        {
            InternalInsertFirstNode(node);
        }
        else
        {
            InternalInsertNodeBefore(_head, node);
            _head = node;
        }
        node._dlclParent = this;
    }

    public DLCLNode<T> AddLast(T value)
    {
        DLCLNode<T> result = new DLCLNode<T>(this, value);
        if (_head == null)
        {
            InternalInsertFirstNode(result);
        }
        else
        {
            InternalInsertNodeBefore(_head, result);
        }
        return result;
    }

    public void AddLast(DLCLNode<T> node)
    {
        ValidateNewNode(node);

        if (_head == null)
        {
            InternalInsertFirstNode(node);
        }
        else
        {
            InternalInsertNodeBefore(_head, node);
        }
        node._dlclParent = this;
    }

    public void Clear()
    {
        DLCLNode<T>? current = _head;
        while (current != null)
        {
            DLCLNode<T> temp = current;
            current = current.Next;
            temp.Invalidate();
        }

        _head = null;
        _count = 0;
    }

    public bool Contains(T value)
    {
        return Find(value) != null;
    }

    public void CopyTo(T[] array, int index)
    {
        ArgumentNullException.ThrowIfNull(array);

        ArgumentOutOfRangeException.ThrowIfNegative(index);

        if (index > array.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, null);
        }

        if (array.Length - index < Count)
        {
            throw new ArgumentException();
        }

        DLCLNode<T>? node = _head;
        if (node != null)
        {
            do
            {
                array[index++] = node!._value;
                node = node._next;
            } while (node != _head);
        }
    }

    public DLCLNode<T>? Find(T value)
    {
        DLCLNode<T>? node = _head;
        EqualityComparer<T> c = EqualityComparer<T>.Default;
        if (node != null)
        {
            if (value != null)
            {
                do
                {
                    if (c.Equals(node!._value, value))
                    {
                        return node;
                    }
                    node = node._next;
                } while (node != _head);
            }
            else
            {
                do
                {
                    if (node!._value == null)
                    {
                        return node;
                    }
                    node = node._next;
                } while (node != _head);
            }
        }
        return null;
    }

    public DLCLNode<T>? FindLast(T value)
    {
        if (_head == null) return null;

        DLCLNode<T>? last = _head._prev;
        DLCLNode<T>? node = last;
        EqualityComparer<T> c = EqualityComparer<T>.Default;
        if (node != null)
        {
            if (value != null)
            {
                do
                {
                    if (c.Equals(node!._value, value))
                    {
                        return node;
                    }

                    node = node._prev;
                } while (node != last);
            }
            else
            {
                do
                {
                    if (node!._value == null)
                    {
                        return node;
                    }
                    node = node._prev;
                } while (node != last);
            }
        }
        return null;
    }

  

    

    public bool Remove(T value)
    {
        DLCLNode<T>? node = Find(value);
        if (node != null)
        {
            InternalRemoveNode(node);
            return true;
        }
        return false;
    }

    public void Remove(DLCLNode<T> node)
    {
        ValidateNode(node);
        InternalRemoveNode(node);
    }

    public void RemoveFirst()
    {
        if (_head == null) { throw new InvalidOperationException(); }
        InternalRemoveNode(_head);
    }

    public void RemoveLast()
    {
        if (_head == null) { throw new InvalidOperationException(); }
        InternalRemoveNode(_head._prev!);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public DLCLEnumerator GetEnumerator() => new DLCLEnumerator(this);
    
    public struct DLCLEnumerator : IEnumerator<T>, IEnumerator
    {
        private readonly DLCL<T> _list;
        private DLCLNode<T>? _node;
        private T? _current;
        private int _index;

        internal DLCLEnumerator(DLCL<T> list)
        {
            _list = list;
            _node = list._head;
            _current = default;
            _index = 0;
        }

        public T Current => _current!;

        object? IEnumerator.Current
        {
            get
            {
                if (_index == 0 || (_index == _list.Count + 1))
                {
                    throw new InvalidOperationException();
                }

                return Current;
            }
        }

        public bool MoveNext()
        {
            if (_node == null)
            {
                _index = _list.Count + 1;
                return false;
            }

            ++_index;
            _current = _node._value;
            _node = _node._next;
            if (_node == _list._head)
            {
                _node = null;
            }
            return true;
        }

        void IEnumerator.Reset()
        {
            _current = default;
            _node = _list._head;
            _index = 0;
        }

        public void Dispose()
        {
        }
    }
}