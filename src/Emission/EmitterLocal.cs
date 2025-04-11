namespace ScrubJay.Reflection.Emission;

[PublicAPI]
public readonly struct EmitterLocal :
#if NET7_0_OR_GREATER
    IEqualityOperators<EmitterLocal, EmitterLocal, bool>,
#endif
    IEquatable<EmitterLocal>,
    IEquatable<LocalVariableInfo>,
    IRenderable
{
    public static bool operator ==(EmitterLocal left, EmitterLocal right) => left.Equals(right);
    public static bool operator !=(EmitterLocal left, EmitterLocal right) => !left.Equals(right);
    
    public readonly Type Type;
    public readonly int Index;
    public readonly bool IsPinned;
    public readonly string? Name;
    
    public bool IsShortForm => Index <= byte.MaxValue;

  
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

    public bool Equals(EmitterLocal emitterLocal)
    {
        return emitterLocal.Index == this.Index &&
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

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is EmitterLocal emitterLocal)
            return Equals(emitterLocal);
        if (obj is LocalVariableInfo localVariableInfo)
            return Equals(localVariableInfo);
        return false;
    }

    public override int GetHashCode() 
        => Hasher.HashMany(Index, Type, IsPinned);

    public void RenderTo(TextBuilder builder)
    {
        builder.Append($"[{Index}] ")
            .AppendIf(IsPinned, "fixed ")
            .AppendType(Type)
            .IfNotNull(Name, static (tb, name) => tb.Append(' ').Append(name));
    }

    public override string ToString() => TextBuilder.Build(RenderTo);
}
