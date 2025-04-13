namespace ScrubJay.Reflection.Comparison;

/// <summary>
/// An <see cref="EqualityComparer{T}"/> for <see cref="Module">Modules</see>
/// </summary>
[PublicAPI]
public sealed class ModuleEqualityComparer : 
    IEqualityComparer<Module>,
    IHasDefault<ModuleEqualityComparer>
{
    public static ModuleEqualityComparer Default { get; } = new();

    public bool Equals(Module? x, Module? y)
    {
        return GetHashCode(x) == GetHashCode(y);
    }

    public int GetHashCode(Module? module)
    {
        if (module is null)
            return Hasher.NullHash;
        return module.MetadataToken;
    }
}