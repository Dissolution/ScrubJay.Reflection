namespace ScrubJay.Reflection;

/// <summary>
/// The access-type of a <see cref="MemberInfo"/>
/// </summary>
[Flags]
public enum Access
{
    None = 0,
    
    Static = 1 << 0,
    Instance = 1 << 1,
    
    Any = Static | Instance,
}