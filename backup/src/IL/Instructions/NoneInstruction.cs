namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class OpCodeNoneInstruction : OpCodeInstruction
{
    public override int Size => OpCode.Size;

    public OpCodeNoneInstruction(OpCode opCode) : base(opCode)
    {
        if (opCode.OperandType != OperandType.InlineNone)
            throw new ArgumentException(null, nameof(opCode));
    }
}