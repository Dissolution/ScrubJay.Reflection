namespace ScrubJay.Reflection;

/// <summary>
/// Represents the visibility modifiers for a Member
/// </summary>
[PublicAPI]
[Flags]
public enum Visibility
{
    None      = 0,
    
    
    Instance  = 1 << 0,
    Static    = 1 << 1,
    
    Public    = 1 << 2,
    Internal  = 1 << 3,
    Protected = 1 << 4,
    Private   = 1 << 5,
    NonPublic = Internal | Protected | Private,
    
    Any = Instance | Static | Public | NonPublic,
}