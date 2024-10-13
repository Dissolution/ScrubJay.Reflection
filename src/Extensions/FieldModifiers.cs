namespace ScrubJay.Reflection.Extensions;

[Flags]
public enum FieldModifiers
{
    None = 0,
    InitOnly = 1 << 0,
    Const = 1 << 1,
    HasDefault = 1 << 2,
    
    Any = InitOnly | Const | HasDefault,
}