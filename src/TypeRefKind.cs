using ScrubJay.Text.Rendering;

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
    [RenderAs("")]
    Default = 1 << 0,
    
    /// <summary>
    /// <c>ref</c>
    /// </summary>
    [RenderAs("ref ")]
    Ref = 1 << 1,
    
    /// <summary>
    /// <c>in</c>
    /// </summary>
    [RenderAs("in ")]
    In = (1 << 2) | Ref,
    
    /// <summary>
    /// <c>out</c>
    /// </summary>
    [RenderAs("out ")]
    Out = (1 << 3) | Ref,

    Any = Default | Ref | In | Out,
}