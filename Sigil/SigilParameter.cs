namespace ScrubJay.Sigil;

/// <summary>
/// Represents a parameter to a decompiled delegate.
/// </summary>
public sealed class SigilParameter
{
    /// <summary>
    /// The index of the parameter.
    /// </summary>
    public int Position { get; private set; }

    /// <summary>
    /// The type of the parameter.
    /// </summary>
    public Type ParameterType { get; private set; }

    internal SigilParameter(int pos, Type type)
    {
        Position = pos;
        ParameterType = type;
    }

    internal static SigilParameter For(ParameterInfo p)
    {
        return new SigilParameter(p.Position, p.ParameterType);
    }

    /// <summary>
    /// Returns a string representation of this Parameter.
    /// </summary>
    public override string ToString()
    {
        return "(" + ParameterType + ") at " + Position;
    }
}
