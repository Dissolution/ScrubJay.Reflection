namespace ScrubJay.Reflection;

/// <summary>
/// Represents the visibility modifiers for a Member
/// </summary>
[PublicAPI]
[Flags]
public enum Visibility
{
    None = 0,

    [RenderAs("instance")]
    Instance = 1 << 0,
    [RenderAs("static")]
    Static = 1 << 1,

    [RenderAs("public")]
    Public = 1 << 2,
    [RenderAs("internal")]
    Internal = 1 << 3,
    [RenderAs("protected")]
    Protected = 1 << 4,
    [RenderAs("private")]
    Private = 1 << 5,
    
    NonPublic = Internal | Protected | Private,

    Any = Instance | Static | Public | NonPublic,
}