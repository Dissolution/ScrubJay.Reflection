namespace ScrubJay.Reflection.OLD.OLD.Expressions;

[Flags]
public enum Scope
{
    // None = 0,
    Self = 1 << 0,
    Children = 1 << 1,
    Descendants = 1 << 2,
}