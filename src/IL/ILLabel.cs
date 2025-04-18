using ScrubJay.Reflection.IL.Instructions;
using ScrubJay.Reflection.Runtime;
using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection.IL;

[PublicAPI]
public readonly struct ILLabel :
#if NET7_0_OR_GREATER
    IEqualityOperators<ILLabel, ILLabel, bool>,
#endif
    IEquatable<ILLabel>,
    IEquatable<Label>,
    IRenderable
{
    public static implicit operator Label(ILLabel ilLabel) => ilLabel.ToLabel();
    public static implicit operator ILLabel(Label label) => new(label);

    public static bool operator ==(ILLabel left, ILLabel right)
        => left.Equals(right);
    public static bool operator !=(ILLabel left, ILLabel right)
        => !left.Equals(right);

    private static readonly Func<int, Label> _newLabel;

    static ILLabel()
    {
        var ctor = Mirror
            .Reflect<Label>()
            .Constructors
            .Parameters<int>()
            .One()
            .SomeOrThrow();

        _newLabel = RuntimeBuilder
            .TryGenerateDelegate<Func<int, Label>>(gen =>
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Newobj, ctor);
                gen.Emit(OpCodes.Ret);
            }).OkOrThrow();
    }

    public static Label CreateLabel(int index) => _newLabel(index);
    
    public readonly int Id;
    public readonly string? Name;
    public readonly ILOffset Offset = ILOffset.Unknown;

    public bool IsShortForm => Id is >= 0 and <= 127;

    public ILLabel(int id, string? name = null)
    {
        this.Id = id;
        this.Name = name;
    }

    public ILLabel(Label label, string? name = null)
    {
        this.Id = label.GetHashCode();
        this.Name = name;
    }

    public ILLabel(int id, ILOffset offset, string? name = null)
    {
        this.Id = id;
        this.Offset = offset;
        this.Name = name;
    }

    public Label ToLabel()
    {
        return _newLabel(Id);
    }

    public bool Equals(ILLabel ilLabel)
    {
        return ilLabel.Id == Id;
    }

    public bool Equals(Label label)
    {
        return label.GetHashCode() == Id;
    }

    public bool Equals(int id)
    {
        return id == Id;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is ILLabel emitterLabel)
            return Equals(emitterLabel);
        if (obj is Label label)
            return Equals(label);
        if (obj is int id)
            return id == Id;
        return false;
    }

    public override int GetHashCode() => Id;

    public void RenderTo<B>(B builder)
        where B : TextBuilderBase<B>
    {
        if (Name is not null)
        {
            builder.Append(Name).Append(':');
        }
        else if (Offset != ILOffset.Unknown)
        {
            Offset.RenderTo(builder);
        }
        else
        {
            builder.Append("IL_").Append(Id, "X4");
        }
    }

    public override string ToString() => TextBuilder.Build(RenderTo);
}