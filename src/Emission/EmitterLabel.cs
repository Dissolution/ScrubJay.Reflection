using ScrubJay.Reflection.Runtime;
using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection.Emission;

[PublicAPI]
public readonly struct EmitterLabel :
#if NET7_0_OR_GREATER
    IEqualityOperators<EmitterLabel, EmitterLabel, bool>,
#endif
    IEquatable<EmitterLabel>,
    IEquatable<Label>,
    IRenderable
{
    public static implicit operator Label(EmitterLabel emitterLabel) => emitterLabel.ToLabel();
    public static implicit operator EmitterLabel(Label label) => new(label);
    
    public static bool operator ==(EmitterLabel left, EmitterLabel right)
        => left.Equals(right);
    public static bool operator !=(EmitterLabel left, EmitterLabel right)
        => !left.Equals(right);

    private static readonly Func<int, Label> _newLabel;

    static EmitterLabel()
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

    public readonly int     Id;
    public readonly string? Name;

    public bool IsShortForm => Id is >= 0 and <= 127;

    public EmitterLabel(int id, string? name = null)
    {
        this.Id = id;
        this.Name = name;
    }

    public EmitterLabel(Label label, string? name = null)
    {
        this.Id = label.GetHashCode();
        this.Name = name;
    }

    public Label ToLabel()
    {
        return _newLabel(Id);
    }

    public bool Equals(EmitterLabel emitterLabel)
    {
        return emitterLabel.Id == Id;
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
        if (obj is EmitterLabel emitterLabel)
            return Equals(emitterLabel);
        if (obj is Label label)
            return Equals(label);
        if (obj is int id)
            return id == Id;
        return false;
    }
    
    public override int GetHashCode() => Id;

    public void RenderTo(TextBuilder builder)
    {
        builder.IfNotNull(Name, Id, 
            static (tb, name) => tb.Append(name).Append(':'),
            static (tb, id) => tb.Append("IL_").Append(id, "X4"));
    }

    public override string ToString() => TextBuilder.Build(RenderTo);
}