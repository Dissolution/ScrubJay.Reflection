namespace ScrubJay.Reflection.Runtime.Emission;

internal sealed class LocalBuilderEqualityComparer : EqualityComparer<LocalBuilder>
{
    public override bool Equals(LocalBuilder? left, LocalBuilder? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.IsPinned == right.IsPinned &&
               left.LocalIndex == right.LocalIndex &&
               left.LocalType == right.LocalType;
    }

    public override int GetHashCode(LocalBuilder? localBuilder)
    {
        if (localBuilder is null) return 0;
        return Hasher.Combine(localBuilder.IsPinned, localBuilder.LocalIndex, localBuilder.LocalType);
    }
}