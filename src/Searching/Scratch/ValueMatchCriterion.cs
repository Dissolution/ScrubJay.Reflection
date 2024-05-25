namespace ScrubJay.Reflection.Searching.Scratch;

public record class ValueMatchCriterion<T> : ICriterion<T>
{
    public T? Value { get; set; } = default;
    public IEqualityComparer<T>? ValueComparer { get; set; } = default;

    public bool Matches(T? str) => (ValueComparer ?? EqualityComparer<T>.Default).Equals(str, Value);
}