namespace ScrubJay.Reflection.Runtime.Emission;

public readonly struct EmitterLabel :
#if NET7_0_OR_GREATER
    IEqualityOperators<EmitterLabel, EmitterLabel, bool>,
    IEqualityOperators<EmitterLabel, Label, bool>,
#endif
    IEquatable<EmitterLabel>,
    IEquatable<Label>
{
    public static bool operator ==(EmitterLabel left, EmitterLabel right) => left.Equals(right);
    public static bool operator !=(EmitterLabel left, EmitterLabel right) => !left.Equals(right);
    public static bool operator ==(EmitterLabel left, Label right) => left.Equals(right);
    public static bool operator !=(EmitterLabel left, Label right) => !left.Equals(right);


    public readonly Label Label;

    public readonly string? Name;

    public int Value => Label.GetHashCode();

    public bool IsShortForm => Label.IsShortForm();

    public EmitterLabel(Label label, string? name = null)
    {
        this.Label = label;
        this.Name = name;
    }

    public bool Equals(EmitterLabel emitterLabel)
    {
        return emitterLabel.Label == Label &&
            string.Equals(emitterLabel.Name, Name, StringComparison.Ordinal);
    }

    public bool Equals(Label label)
    {
        return label == Label;
    }

    public override bool Equals(object? obj) => obj switch
    {
        EmitterLabel emitterLabel => Equals(emitterLabel),
        Label label => Equals(label),
        _ => false,
    };

    public override int GetHashCode() => Label.GetHashCode();

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(Name))
        {
            return $"{Name}:";
        }
        else
        {
            return $"lbl_{Label.GetHashCode()}";
        }
    }
}