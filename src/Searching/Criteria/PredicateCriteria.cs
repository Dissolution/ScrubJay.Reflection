namespace ScrubJay.Reflection.Searching.Criteria;

public sealed class PredicateCriteria<T> : ICriteria<T>
{
    private readonly Func<T?, bool> _predicate;

    public PredicateCriteria(Func<T?, bool> predicate)
    {
        _predicate = predicate;
    }

    public bool Matches(T? value) => _predicate(value);
}