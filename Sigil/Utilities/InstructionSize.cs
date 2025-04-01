namespace ScrubJay.Sigil.Utilities;

public static class InstructionSize
{
    public static int Get(OpCode op, SigilLabel[]? labels = null)
    {
        var baseSize = op.Size;

        int operandSize = op.OperandType switch
        {
            OperandType.InlineBrTarget => 4,
            OperandType.InlineField => 4,
            OperandType.InlineI => 4,
            OperandType.InlineI8 => 8,
            OperandType.InlineMethod => 4,
            OperandType.InlineNone => 0,
            OperandType.InlineR => 8,
            OperandType.InlineSig => 4,
            OperandType.InlineString => 4,
            OperandType.InlineSwitch => 4 + (labels!.Length * 4),
            OperandType.InlineTok => 4,
            OperandType.InlineType => 4,
            OperandType.InlineVar => 2,
            OperandType.ShortInlineBrTarget => 1,
            OperandType.ShortInlineI => 1,
            OperandType.ShortInlineR => 4,
            OperandType.ShortInlineVar => 1,
            _ => throw new Exception("Unexpected operand type [" + op.OperandType + "]"),
        };

        return baseSize + operandSize;
    }

    public static int BeginCatchBlock() => Get(OpCodes.Leave);
    public static int EndCatchBlock() => Get(OpCodes.Leave);

    public static int BeginExceptionBlock() => 0;
    public static int EndExceptionBlock() => 0;

    public static int BeginFinallyBlock() => Get(OpCodes.Leave);
    public static int EndFinallyBlock() => Get(OpCodes.Endfinally);

    public static int DeclareLocal() => 0;
    public static int DefineLabel() => 0;
    public static int MarkLabel() => 0;
}
