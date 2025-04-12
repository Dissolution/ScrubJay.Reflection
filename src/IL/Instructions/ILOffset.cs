namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
[StructLayout(LayoutKind.Auto, Size = 4)]
public readonly struct ILOffset : IRenderable
{
    public static implicit operator ILOffset(int offset) => new(offset);
    public static implicit operator int(ILOffset ilOffset) => ilOffset._offset;

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
        _offset = offset;
    }

    public void RenderTo<B>(B builder) 
        where B : TextBuilderBase<B>
    {
        builder.Append("IL_")
            .If(_offset, static off => off >= 0,
                static (tb, off) => tb.Append(off, "X4"),
                static (tb, off) => tb.Append("????"));
    }

    public override string ToString() => TextBuilder.Build(RenderTo);
}