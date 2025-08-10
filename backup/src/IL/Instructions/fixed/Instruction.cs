namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public abstract class Instruction :
#if NET7_0_OR_GREATER
    IEqualityOperators<Instruction, Instruction, bool>,
#endif
    IEquatable<Instruction>,
    IRenderable
{
    public static bool operator ==(Instruction? left, Instruction? right)
        => Equate.EquatableValues<Instruction>(left, right);

    public static bool operator !=(Instruction? left, Instruction? right)
        => !Equate.EquatableValues<Instruction>(left, right);

    public abstract int Size { get; }

    protected Instruction()
    {
    }

    internal virtual void SetOffset(ILOffset offset) { }
    
    public virtual bool Equals(Instruction? other)
    {
        return ReferenceEquals(this, other);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Instruction instruction)
            return Equals(instruction);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Hash<Type>(this.GetType());
    }

    public virtual void RenderTo(TextBuilder builder)
    {
        builder.RenderType(this);
    }

    public sealed override string ToString()
        => TextBuilder.Build(RenderTo);
}