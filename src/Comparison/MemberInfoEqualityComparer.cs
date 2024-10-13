namespace ScrubJay.Reflection.Comparison;

[PublicAPI]
public sealed class MemberInfoEqualityComparer : EqualityComparer<MemberInfo>
{
    public static MemberInfoEqualityComparer Instance { get; } = new();
    
    public override bool Equals(MemberInfo? x, MemberInfo? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return ModuleEqualityComparer.Instance.Equals(x.Module, y.Module) &&
            x.MetadataToken == y.MetadataToken;
    }
    
    public override int GetHashCode(MemberInfo? member)
    {
        if (member is null) return 0;
        return Hasher.Combine(member.Module.MetadataToken, member.MetadataToken);
    }
}