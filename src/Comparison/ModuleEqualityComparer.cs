namespace ScrubJay.Reflection.Comparison;

/// <summary>
/// An <see cref="EqualityComparer{T}"/> for <see cref="Module">Modules</see>
/// </summary>
[PublicAPI]
public sealed class ModuleEqualityComparer : EqualityComparer<Module>
{
    public static ModuleEqualityComparer Instance { get; } = new();

    public override bool Equals(Module? x, Module? y)
    {
        if (ReferenceEquals(x, y)) 
            return true;
        if (x is null || y is null) 
            return false;
        return x.MetadataToken == y.MetadataToken;
    }

    public override int GetHashCode(Module? module)
    {
        if (module is null)
            return 0;
        return module.MetadataToken;
    }
}