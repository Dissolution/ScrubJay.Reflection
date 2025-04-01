namespace ScrubJay.Reflection;

/// <summary>
/// The access type of a Member
/// </summary>
[Flags]
public enum Access
{
    None = 0,
    Static = 1 << 0,
    Instance = 1 << 1,
    Any = Static | Instance,
}