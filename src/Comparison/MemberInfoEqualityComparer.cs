namespace ScrubJay.Reflection.Comparison;

public class ModuleEqualityComparer : EqualityComparer<Module>
{

    public override bool Equals(Module? x, Module? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.MetadataToken == y.MetadataToken;
    }
    public override int GetHashCode(Module? module)
    {
        if (module is null) return 0;
        return module.MetadataToken;
    }
}

public class MemberInfoEqualityComparer : EqualityComparer<MemberInfo>
{
    public override bool Equals(MemberInfo? x, MemberInfo? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return ModuleEqualityComparer.Default.Equals(x.Module, y.Module) &&
            x.MetadataToken == y.MetadataToken;
    }
    
    public override int GetHashCode(MemberInfo? member)
    {
        if (member is null) return 0;
        return Hasher.Combine(member.Module.MetadataToken, member.MetadataToken);
    }
}