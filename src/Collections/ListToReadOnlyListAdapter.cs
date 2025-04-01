namespace ScrubJay.Reflection.Collections;

public sealed class ListToReadOnlyListAdapter<T> : IReadOnlyList<T>
{
    private readonly IList<T> _list;

    public int Count => _list.Count;

    public T this[int index] => _list[index];

    public ListToReadOnlyListAdapter(IList<T> list)
    {
        _list = list;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
}