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