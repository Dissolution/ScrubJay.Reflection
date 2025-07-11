namespace ScrubJay.Reflection.IL.LabelOffSetManagement;

[PublicAPI]
public sealed class ILLabel :
    IEquatable<ILLabel>,
    IRenderable
{
    public int Id { get; }
    public string? Name { get; internal set; } = null;
    public ILOffset Offset { get; internal set; } = ILOffset.Unknown;

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

    public bool Equals(ILLabel? other)
        => ReferenceEquals(this, other);
    
    public override bool Equals([NotNullWhen(true)] object? obj)
        => ReferenceEquals(this, obj);

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
        builder
            .IfNotNull(Name, static (tb, name) => tb.Append(' ').Append(name))
            .Append(':');
    }

    public override string ToString() => TextBuilder.Build(RenderTo);
}