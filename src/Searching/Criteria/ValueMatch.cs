namespace ScrubJay.Reflection.Searching.Criteria;

public record class ValueMatchCriterion<T> : ICriterion<T>
{
    public T? Value { get; set; } = default;
    public IEqualityComparer<T>? ValueComparer { get; set; } = default;

    public bool Matches(T? value) => (ValueComparer ?? EqualityComparer<T>.Default).Equals(value!, Value!);
}