using System.Reflection.Emit;

namespace ScrubJay.Reflection.OLD.OLD.Runtime.Emission.Instructions;

public record class OpCodeInstruction : Instruction
{
    public OpCode OpCode { get; }

    public override int Size
    {
        get
        {
            int size = OpCode.Size;
            if (OpCode.OperandType != OperandType.InlineNone)
                throw new InvalidOperationException();
            return size;
        }
    }

    public OpCodeInstruction(OpCode opCode)
    {
        this.OpCode = opCode;
    }

    public override string ToString() => OpCode.Name ?? OpCode.GetType().NameOf();
}