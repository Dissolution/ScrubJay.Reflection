namespace ScrubJay.Reflection.IL;

[PublicAPI]
public readonly struct ILLabel :
#if NET7_0_OR_GREATER
    IEqualityOperators<ILLabel, ILLabel, bool>,
#endif
    IEquatable<ILLabel>,
    IRenderable
{
    public static bool operator ==(ILLabel left, ILLabel right) => left.Equals(right);
    public static bool operator !=(ILLabel left, ILLabel right) => !left.Equals(right);

    public readonly int Id;
    public readonly string? Name;
    public readonly ILOffset Offset;

    public ILLabel(int id, string? name = null)
    {
        this.Id = id;
        this.Name = name;
    }

    public ILLabel(int id, ILOffset offset, string? name = null)
    {
        this.Id = id;
        this.Offset = offset;
        this.Name = name;
    }

    public bool Equals(ILLabel label) => label.Offset == this.Offset || label.Id == this.Id;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is ILLabel label)
            return Equals(label);
        if (obj is ILOffset offset)
            return offset == this.Offset;
        return false;
    }

    public override int GetHashCode() => Throw.NotSupported<int>();

    public void RenderTo(TextBuilder builder)
    {
        if (Offset.IsUnknown)
        {
            builder.Append($"<{Id}>");
        }
        else
        {
            builder.Render(Offset);
        }

        builder.IfNotNull(Name, static (tb, name) => tb.Append(' ').Append(name));
    }

    public override string ToString() => TextBuilder.Build(RenderTo);
}