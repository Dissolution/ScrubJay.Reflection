namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

[Flags]
public enum CompareOp
{
    NotEqual = 0,
    Equal = 1 << 1,
    GreaterThan = 1 << 2,
    LessThan = 1 << 3,
    GreaterThanOrEqual = GreaterThan | Equal,
    LessThanOrEqual = LessThan | Equal,
}