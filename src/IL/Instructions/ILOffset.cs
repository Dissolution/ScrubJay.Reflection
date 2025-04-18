namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
[StructLayout(LayoutKind.Auto, Size = 4)]
public readonly struct ILOffset :
#if NET7_0_OR_GREATER
    IEqualityOperators<ILOffset, ILOffset, bool>,
#endif
    IEquatable<ILOffset>,
    IRenderable
{
    public static implicit operator ILOffset(int offset) => new(offset);
    public static implicit operator int(ILOffset ilOffset) => ilOffset._offset;

    public static bool operator ==(ILOffset left, ILOffset right) => left.Equals(right);
    public static bool operator !=(ILOffset left, ILOffset right) => !left.Equals(right);

    public static ILOffset operator +(ILOffset ilOffset, int i32)
    {
        if (ilOffset == Unknown)
            return Unknown;
        return new(ilOffset._offset + i32);
    }

    public const int SIZE = 4;

    public static readonly ILOffset Unknown = new(int.MinValue);

    
    private readonly int _offset;

    public ILOffset(int offset)
    {
        if (offset < 0)
        {
            _offset = int.MinValue;
        }
        else
        {
            _offset = offset;
        }
    }

    public void RenderTo<B>(B builder)
        where B : TextBuilderBase<B>
    {
        builder.Append("IL_")
            .If(_offset, static off => off >= 0,
                static (tb, off) => tb.Append(off, "X4"),
                static (tb, _) => tb.Append("????"));
    }

    public bool Equals(ILOffset ilOffset)
    {
        return _offset == ilOffset._offset;
    }

    public bool Equals(int offset)
    {
        if (offset < 0)
            return _offset == int.MinValue;
        return _offset == offset;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is ILOffset ilOffset)
            return Equals(ilOffset);
        if (obj is int offset)
            return Equals(offset);
        return false;
    }
    public override int GetHashCode() => _offset;

    public override string ToString() => TextBuilder.Build(RenderTo);
}