namespace ScrubJay.Reflection.IL;

[PublicAPI]
public readonly struct ILLocal :
#if NET7_0_OR_GREATER
    IEqualityOperators<ILLocal, ILLocal, bool>,
#endif
    IEquatable<ILLocal>,
    IEquatable<LocalVariableInfo>,
    IRenderable
{
    public static bool operator ==(ILLocal left, ILLocal right) => left.Equals(right);
    public static bool operator !=(ILLocal left, ILLocal right) => !left.Equals(right);
    
    public readonly Type Type;
    public readonly int Index;
    public readonly bool IsPinned;
    public readonly string? Name;
    
    public bool IsShortForm => Index <= byte.MaxValue;
  
    public ILLocal(int index, Type type, bool isPinned = false, string? name = null)
    {
        this.Index = index;
        this.Type = type;
        this.IsPinned = isPinned;
        this.Name = name;
    }
    
    public ILLocal(LocalVariableInfo localVariableInfo, string? name = null)
    {
        this.Index = localVariableInfo.LocalIndex;
        this.Type = localVariableInfo.LocalType!;
        this.IsPinned = localVariableInfo.IsPinned;
        this.Name = name;
    }

    public bool Equals(ILLocal ilLocal)
    {
        return ilLocal.Index == this.Index &&
            ilLocal.Type == this.Type &&
            ilLocal.IsPinned == this.IsPinned;
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
        if (obj is ILLocal emitterLocal)
            return Equals(emitterLocal);
        if (obj is LocalVariableInfo localVariableInfo)
            return Equals(localVariableInfo);
        return false;
    }

    public override int GetHashCode() 
        => Hasher.HashMany(Index, Type, IsPinned);

    public void RenderTo<B>(B builder) 
        where B : TextBuilderBase<B>
    {
        builder//.Append($"[{Index}] ")
            .AppendIf(IsPinned, "fixed ")
            .AppendType(Type)
            .IfNotNull(Name, static (tb, name) => tb.Append(' ').Append(name));
    }

    public override string ToString() => TextBuilder.Build(RenderTo);
}
