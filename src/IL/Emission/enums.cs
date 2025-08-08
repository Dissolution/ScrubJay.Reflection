namespace ScrubJay.Reflection.IL.Emission;

[PublicAPI]
[Flags]
public enum CompareOp
{
    NotEqual = 0,
    Equal = 1 << 0,
    LessThan = 1 << 1,
    GreaterThan = 1 << 2,
    LessThanOrEqual = LessThan | Equal,
    GreaterThanOrEqual = GreaterThan | Equal,
    Unconditional = Equal | LessThan | GreaterThan,
}

[PublicAPI]
public enum MathOp
{
    Add,
    Subtract,
    Multiply,
    Divide,
    Modulo,
}

[PublicAPI]
public enum BitwiseOp
{
    And,
    Neg,
    Not,
    Or,
    Shl,
    Shr,
    ShrUn,
    Xor,
}