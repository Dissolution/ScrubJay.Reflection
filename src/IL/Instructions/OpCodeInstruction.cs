namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public abstract class OpCodeInstruction : Instruction
{
    public OpCode OpCode { get; }

    /*
    public override int Size
    {
        get
        {
            
            int size = OpCode.Size;

            switch (OpCode.OperandType)
            {
                case OperandType.InlineI8:
                case OperandType.InlineR:
                    size += 8;
                    break;
                case OperandType.InlineBrTarget:
                case OperandType.InlineField:
                case OperandType.InlineI:
                case OperandType.InlineMethod:
                case OperandType.InlineSig:
                case OperandType.InlineString:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.ShortInlineR:
                    size += 4;
                    break;
                case OperandType.InlineVar:
                    size += 2;
                    break;
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                    size += 1;
                    break;

                case OperandType.InlineSwitch:
                    throw new InvalidOperationException("OpCodeInstruction cannot handle InlineSwitch");

#pragma warning disable CS0618 // Type or member is obsolete
                case OperandType.InlinePhi:
#pragma warning restore CS0618
                case OperandType.InlineNone:
                default:
                    break; // no change
            }

            return size;
        }
    }
    */
    
    public bool IsShort
    {
        get
        {
            return OpCode.OperandType is >= OperandType.ShortInlineBrTarget and <= OperandType.ShortInlineVar;
        }
    }

    public bool IsStandard
    {
        get
        {
            return OpCode.OperandType != OperandType.InlineNone &&
                OpCode.OperandType <= OperandType.InlineVar;
        }
    }
    
    protected internal OpCodeInstruction(OpCode opCode)
    {
        this.OpCode = opCode;
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(base.RenderTo)
            // maximum OpCode.Name is 13, so this neatly sets us up with one space before the next render
            .Align(OpCode.Name.AsSpan(), 14, alignment: Alignment.Left);
    }
}


[PublicAPI]
public sealed class OpCodeNoneInstruction : OpCodeInstruction
{
    public override int Size => OpCode.Size;

    public OpCodeNoneInstruction(OpCode opCode) : base(opCode)
    {
        Debug.Assert(opCode.OperandType == OperandType.InlineNone);
    }
}