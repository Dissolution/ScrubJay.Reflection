namespace ScrubJay.Reflection;

/// <summary>
/// The reference kind for a <see cref="Type"/>, <see cref="ParameterInfo"/>
/// </summary>
[PublicAPI]
[Flags]
public enum TypeRefKind
{
    /// <summary>
    /// Default referencing (copy value, ref class)
    /// </summary>
    Default = 1 << 0,
    
    /// <summary>
    /// <c>ref</c>
    /// </summary>
    Ref = 1 << 1,
    /// <summary>
    /// <c>in</c>
    /// </summary>
    In = (1 << 2) | Ref,
    /// <summary>
    /// <c>out</c>
    /// </summary>
    Out = (1 << 3) | Ref,

    Any = Default | Ref | In | Out,
}

public static class TypeRefKindExtensions
{
    public static string AsString(this TypeRefKind kind)
    {
        return kind switch
        {
            TypeRefKind.In => "in ",
            TypeRefKind.Out => "out ",
            TypeRefKind.Ref => "ref ",
            _ => string.Empty,
        };
    }
}