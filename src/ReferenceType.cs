namespace ScrubJay.Reflection;

/// <summary>
/// The reference type for a <see cref="ParameterInfo"/>
/// </summary>
[Flags]
public enum ReferenceType
{
    None = 0,
    
    Default = 1 << 0,
    Ref = 1 << 1,
    In = 1 << 2 | Ref,
    Out = 1 << 3 | Ref,
    
    Any = Default | Ref | In | Out,
}