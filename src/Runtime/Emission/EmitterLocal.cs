namespace ScrubJay.Reflection.Runtime.Emission;

/// <summary>
/// A local variable managed by an Emitter
/// </summary>
[PublicAPI]
public sealed class EmitterLocal : LocalVariableInfo,
#if NET7_0_OR_GREATER
    IEqualityOperators<EmitterLocal, EmitterLocal, bool>,
    IEqualityOperators<EmitterLocal, LocalVariableInfo, bool>,
#endif
    IEquatable<EmitterLocal>,
    IEquatable<LocalVariableInfo>
{
    public static bool operator ==(EmitterLocal? left, EmitterLocal? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }
    public static bool operator !=(EmitterLocal? left, EmitterLocal? right) => !(left == right);
    public static bool operator ==(EmitterLocal? left, LocalVariableInfo? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(EmitterLocal? left, LocalVariableInfo? right) => !(left == right);
    
    
    public string? Name { get; }
    public override int LocalIndex { get; }
    public override Type? LocalType { get; }
    public override bool IsPinned { get; }
    public bool IsShortForm => LocalIndex <= byte.MaxValue;

    public EmitterLocal(int index, Type type, bool isPinned = false, string? name = null)
    {
        this.LocalIndex = index;
        this.LocalType = type;
        this.IsPinned = isPinned;
        this.Name = name;
    }
    public EmitterLocal(LocalVariableInfo localVariableInfo, string? name = null)
    {
        this.LocalIndex = localVariableInfo.LocalIndex;
        this.LocalType = localVariableInfo.LocalType;
        this.IsPinned = localVariableInfo.IsPinned;
        this.Name = name;
    }

    public bool Equals(EmitterLocal? emitterLocal)
    {
        return emitterLocal is not null &&
            emitterLocal.LocalIndex == this.LocalIndex &&
            emitterLocal.LocalType == this.LocalType &&
            emitterLocal.IsPinned == this.IsPinned;
    }
    
    public bool Equals(LocalVariableInfo? localVariableInfo)
    {
        return localVariableInfo is not null &&
            localVariableInfo.LocalIndex == this.LocalIndex &&
            localVariableInfo.LocalType == this.LocalType &&
            localVariableInfo.IsPinned == this.IsPinned;
    }

    public override bool Equals(object? obj) => obj switch
    {
        EmitterLocal emitterLocal => Equals(emitterLocal),
        LocalVariableInfo localVariableInfo => Equals(localVariableInfo),
        _ => false,
    };
    
    public override int GetHashCode()
    {
        return LocalIndex;
    }
    
    public override string ToString()
    {
        return $"[{LocalIndex}]: {(IsPinned ? "fixed " : "")}{LocalType.NameOf()} {Name ?? "???"}";
    }
}