namespace ScrubJay.Reflection.Searching.Predication;

public class ArrayEqualityCriteria<T> : EqualityCriteria<T[]>
{
    public IEqualityComparer<T>? ValueComparer { get; set; }

    public ArrayEqualityCriteria()
    {
    }

    [SetsRequiredMembers]
    public ArrayEqualityCriteria(T[]? value) : base(value)
    {
    }

    public override bool Matches(T[]? value)
    {
        return Sequence.Equal(Value, value, ValueComparer);
    }
}