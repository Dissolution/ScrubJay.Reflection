namespace ScrubJay.Reflection.Runtime.Emission;

/// <summary>
/// An <see cref="EqualityComparer{T}"/> for <see cref="LocalBuilder"/>s
/// </summary>
public sealed class LocalBuilderEqualityComparer : EqualityComparer<LocalBuilder>
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

/// <summary>
/// An <see cref="EqualityComparer{T}"/> for <see cref="LocalVariableInfo"/>s
/// </summary>
public sealed class LocalVariableInfoEqualityComparer : EqualityComparer<LocalVariableInfo>
{
    public override bool Equals(LocalVariableInfo? left, LocalVariableInfo? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.IsPinned == right.IsPinned &&
            left.LocalIndex == right.LocalIndex &&
            left.LocalType == right.LocalType;
    }

    public override int GetHashCode(LocalVariableInfo? localVariableInfo)
    {
        if (localVariableInfo is null) return 0;
        return Hasher.Combine(localVariableInfo.IsPinned, localVariableInfo.LocalIndex, localVariableInfo.LocalType);
    }
}