namespace ScrubJay.Reflection.Runtime.Emission;

[PublicAPI]
public sealed record class EmitterLocal :
#if NET7_0_OR_GREATER
    IEqualityOperators<EmitterLocal, LocalVariableInfo, bool>,
#endif
    IEquatable<LocalVariableInfo>
{
    public Type Type { get; }
    public int Index { get; }
    public bool IsPinned { get; }
    public string? Name { get; }
    public bool IsShortForm => Index <= byte.MaxValue;

    public static bool operator ==(EmitterLocal? left, LocalVariableInfo? right) => left is not null && left.Equals(right);
    public static bool operator !=(EmitterLocal? left, LocalVariableInfo? right) => left is null ? right is null : !left.Equals(right);

    public EmitterLocal(int index, Type type, bool isPinned = false, string? name = null)
    {
        this.Index = index;
        this.Type = type;
        this.IsPinned = isPinned;
        this.Name = name;
    }
    public EmitterLocal(LocalVariableInfo localVariableInfo, string? name = null)
    {
        this.Index = localVariableInfo.LocalIndex;
        this.Type = localVariableInfo.LocalType;
        this.IsPinned = localVariableInfo.IsPinned;
        this.Name = name;
    }

    public bool Equals(EmitterLocal? emitterLocal)
    {
        return emitterLocal is not null &&
            emitterLocal.Index == this.Index &&
            emitterLocal.Type == this.Type &&
            emitterLocal.IsPinned == this.IsPinned;
    }

    public bool Equals(LocalVariableInfo? localVariableInfo)
    {
        return localVariableInfo is not null &&
            localVariableInfo.LocalIndex == this.Index &&
            localVariableInfo.LocalType == this.Type &&
            localVariableInfo.IsPinned == this.IsPinned;
    }

    public override int GetHashCode() => Hasher.HashMany(Index, Type, IsPinned);

    public override string ToString()
    {
        return TextBuilder.New
            .Append('[').Append(Index).Append("]: ")
            .AppendIf(IsPinned, "fixed ")
            .AppendType(Type)
            .If(Validate.IsNotEmpty(Name), static (tb, name) => tb.Append(' ').Append(name))
            .ToStringAndDispose();
    }
}
