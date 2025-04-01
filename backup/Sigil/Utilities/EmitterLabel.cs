#nullable enable

using System.Numerics;
using ScrubJay.Comparison;
using ScrubJay.Functional;

namespace ScrubJay.Sigil.Utilities;

[PublicAPI]
public sealed class EmitterLabel :
#if NET7_0_OR_GREATER
    IEqualityOperators<EmitterLabel, EmitterLabel, bool>,
    IEqualityOperators<EmitterLabel, Label, bool>,
#endif
    IEquatable<EmitterLabel>,
    IEquatable<Label>
{
    public static bool operator ==(EmitterLabel? left, EmitterLabel? right) => Equate.EquatableValues(left, right);
    public static bool operator !=(EmitterLabel? left, EmitterLabel? right) => !Equate.EquatableValues<EmitterLabel, EmitterLabel>(left, right);
    public static bool operator ==(EmitterLabel? left, Label right) => Equate.EquatableValues<EmitterLabel, Label>(left, right);
    public static bool operator !=(EmitterLabel? left, Label right) => !Equate.EquatableValues<EmitterLabel, Label>(left, right);


    public int Id { get; }
    public string? Name { get; }
    public Option<int> MarkedPosition { get; set; } = Option<int>.None();
    public bool IsShortForm => Id is >= 0 and <= 127;

    public EmitterLabel(int id, string? name = null)
    {
        Id = id;
        Name = name;
    }

    public EmitterLabel(Label label, string? name = null)
    {
        Id = label.GetHashCode();
        Name = name;
    }

    /*public System.Reflection.Emit.Label ToLabel()
    {
        return EmissionHelper.NewLabel(Id);
    }*/

    public bool Equals(EmitterLabel? emitterLabel)
    {
        return emitterLabel is not null && this.Id == emitterLabel.Id;
    }

    public bool Equals(Label label)
    {
        return label.GetHashCode() == Id;
    }

    public override int GetHashCode() => Id;

    public override string ToString()
    {
        return Name ?? $"lbl{Id}";
    }
}
