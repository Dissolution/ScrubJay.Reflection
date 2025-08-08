namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public abstract class VariableInstruction : OpCodeInstruction
{
    public int Index { get; }

    public override sealed int Size
    {
        get
        {
            if (OpCode.OperandType == OperandType.InlineVar)
                return OpCode.Size + sizeof(ushort);
            if (OpCode.OperandType == OperandType.ShortInlineVar)
                return OpCode.Size + sizeof(byte);
            Debug.Assert(OpCode.OperandType == OperandType.InlineNone);
            return OpCode.Size;
        }
    }

    protected VariableInstruction(OpCode opCode, int index) : base(opCode)
    {
        switch (opCode.OperandType)
        {
            case OperandType.InlineNone:
            {
                if (index < 0 || index > 3)
                    throw new ArgumentException(null, nameof(index));
                break;
            }
            case OperandType.InlineVar:
            {
                if ((uint)index > (uint)ushort.MaxValue)
                    throw new ArgumentException(null, nameof(index));
                break;
            }
            case OperandType.ShortInlineVar:
            {
                if ((uint)index > (uint)byte.MaxValue)
                    throw new ArgumentException(null, nameof(index));
                break;
            }
            default:
                throw new ArgumentException(null, nameof(opCode));
        }

        this.Index = index;
    }
}