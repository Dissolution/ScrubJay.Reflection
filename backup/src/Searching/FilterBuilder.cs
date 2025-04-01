using ScrubJay.Fluent;

namespace ScrubJay.Reflection.Searching;

public abstract class FilterBuilder<TB, T> : FluentBuilder<TB>
    where TB : FilterBuilder<TB, T>
{
    protected internal readonly List<T> _items;

    protected FilterBuilder(IEnumerable<T> items)
    {
        _items = new(items);
    }

    protected internal IEnumerable<TN> SelectItems<TN>(Func<T, TN> selector) => _items.Select(selector);
    protected internal IEnumerable<TN> ItemsOfType<TN>() => _items.OfType<TN>();

    public TB Where(Func<T, bool> predicate)
    {
        _ = _items.RemoveAll(item => !predicate(item));
        return _builder;
    }

    public T First() => _items.First();

    public T? FirstOrDefault() => _items.FirstOrDefault();

    public List<T> ToList() => _items.ToList();

    public IEnumerable<T> Enumerate() => _items;
}
