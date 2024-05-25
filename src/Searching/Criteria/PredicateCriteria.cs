namespace ScrubJay.Reflection.Searching.Criteria;

public sealed record class PredicateCriteria<T> : Criteria<T>, ICriteria<T>
{
    private readonly Func<T?, bool> _predicate;

    public PredicateCriteria(Func<T?, bool> predicate)
    {
        _predicate = predicate;
    }

    public override bool Matches(T? value) => _predicate(value);
}