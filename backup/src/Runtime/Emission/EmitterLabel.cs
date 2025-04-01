namespace ScrubJay.Reflection.Runtime.Emission;

[PublicAPI]
public sealed record class EmitterLabel :
#if NET7_0_OR_GREATER
    IEqualityOperators<EmitterLabel, Label, bool>,
#endif
    IEquatable<Label>
{
    public static bool operator ==(EmitterLabel? left, Label right) => left is not null && left.Equals(right);
    public static bool operator !=(EmitterLabel? left, Label right) => left is null || !left.Equals(right);


    public int Id { get; }
    public string? Name { get; }

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
        return EmissionHelper.NewLabel(Id);
    }

    public bool Equals(Label label)
    {
        return label.GetHashCode() == this.Id;
    }

    public override int GetHashCode() => Id;

    public override string ToString()
    {
        return Name ?? $"lbl{Id}";
    }
}