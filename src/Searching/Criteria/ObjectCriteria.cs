namespace ScrubJay.Reflection.Searching.Criteria;

public record class ObjectCriteria : Criteria, ICriteria<object>
{
    public TypeCriteria? Type { get; init; } = null;
    public PredicateCriteria<object?>? Predicate { get; init; } = null;

    public bool Matches(object? obj)
    {
        if (Type is not null && (obj is null || !Type.Matches(obj.GetType())))
            return false;
        if (Predicate is not null && !Predicate.Matches(obj))
            return false;
        return true;
    }
}