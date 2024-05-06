using Jay.Reflection.Building.Emission;
using ScrubJay.Comparison;

namespace ScrubJay.Reflection.Runtime.Emission;

public sealed class EmitterLocal : 
#if NET7_0_OR_GREATER
    IEqualityOperators<EmitterLocal, EmitterLocal, bool>,
    IEqualityOperators<EmitterLocal, LocalBuilder, bool>,
    IEqualityOperators<EmitterLocal, LocalVariableInfo, bool>,
#endif
    IEquatable<EmitterLocal>, 
    IEquatable<LocalBuilder>,
    IEquatable<LocalVariableInfo>
{
    public static implicit operator LocalBuilder(EmitterLocal local) => local._localBuilder;

    public static bool operator ==(EmitterLocal? left, EmitterLocal? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }
    public static bool operator !=(EmitterLocal? left, EmitterLocal? right) => !(left == right);
    public static bool operator ==(EmitterLocal? left, LocalBuilder? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(EmitterLocal? left, LocalBuilder? right) => !(left == right);
    public static bool operator ==(EmitterLocal? left, LocalVariableInfo? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(EmitterLocal? left, LocalVariableInfo? right) => !(left == right);

    
    private readonly LocalBuilder _localBuilder;
    private readonly string? _name;

    public LocalBuilder Local => _localBuilder;
    public string? Name => _name;

    public int Index => _localBuilder.LocalIndex;
    public Type Type => _localBuilder.LocalType;
    public bool IsPinned => _localBuilder.IsPinned;
    public bool IsShortForm => _localBuilder.IsShortForm();

    public EmitterLocal(LocalBuilder local, string? name = null)
    {
        _localBuilder = local;
        _name = name;
    }

    public bool Equals(EmitterLocal? emitterLocal)
    {
        if (emitterLocal is null) return false;
        return LocalBuilderEqualityComparer.Default.Equals(_localBuilder, emitterLocal._localBuilder);
    }

    public bool Equals(LocalBuilder? localBuilder)
    {
        if (localBuilder is null) return false;
        return LocalBuilderEqualityComparer.Default.Equals(_localBuilder, localBuilder);
    }

    public bool Equals(LocalVariableInfo? localVariableInfo)
    {
        return localVariableInfo is not null &&
               localVariableInfo.LocalIndex == _localBuilder.LocalIndex &&
               localVariableInfo.LocalType == _localBuilder.LocalType &&
               localVariableInfo.IsPinned == _localBuilder.IsPinned;
    }

    public override bool Equals(object? obj) => obj switch
    {
        EmitterLocal emitterLocal => Equals(emitterLocal),
        LocalBuilder localBuilder => Equals(localBuilder),
        LocalVariableInfo localVariableInfo => Equals(localVariableInfo),
        _ => false,
    };
    
    public override int GetHashCode()
    {
        return Index;
    }
    
    public override string ToString()
    {
        return $"[{Index}]: {(IsPinned ? "fixed " : "")}{Type} {Name ?? "???"}";
    }
}