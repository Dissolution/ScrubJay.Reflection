namespace ScrubJay.Reflection;

[Flags]
public enum RefKind
{
    None = 1 << 0,
    Ref = 1 << 1,
    In = 1 << 2 | Ref,
    Out = 1 << 3 | Ref,
    
    Any = None | Ref | In | Out,
}