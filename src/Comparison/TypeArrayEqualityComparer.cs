namespace ScrubJay.Reflection.Comparison;

public sealed class TypeArrayEqualityComparer : IEqualityComparer<Type[]>, IHasDefault<TypeArrayEqualityComparer>
{
    public static TypeArrayEqualityComparer Default { get; } = new();

    public bool Equals(Type[]? x, Type[]? y)
    {
        return Sequence.Equal(x, y);
    }
    
    public int GetHashCode(Type[]? types)
    {
        return Hasher.HashMany<Type>(types);
    }
}