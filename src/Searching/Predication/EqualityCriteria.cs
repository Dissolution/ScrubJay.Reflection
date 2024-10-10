namespace ScrubJay.Reflection.Searching.Predication;

public class EqualityCriteria<T> : ICriteria<T?>, IEquatable<T>
{
    public required T? Value { get; init; }
    
    public EqualityCriteria() { }

    [SetsRequiredMembers]
    public EqualityCriteria(T? value)
    {
        this.Value = value;
    }

    public virtual bool Matches(T? value)
    {
        return EqualityComparer<T>.Default.Equals(this.Value!, value!);
    }

    public bool Equals(T? other) => Matches(other);

    public override bool Equals(object? obj)
    {
        if (obj is T value)
            return Matches(value);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.GetHashCode<T>(Value);
    }

    public override string ToString()
    {
        return $"Value = {Value}";
    }
}

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
        return Equate.Sequence(Value, value, ValueComparer);
    }
}