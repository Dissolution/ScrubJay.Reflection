namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class OpCodeBranchInstruction : OpCodeInstruction
{
    public int Delta { get; }
    public int DeltaSize { get; }
    
    public ILOffset TargetOffset { get; }
    public ILLabel? Label { get; set; }

    public override sealed int Size => OpCode.Size + DeltaSize;
  
    public OpCodeBranchInstruction(OpCode opCode, int delta)
        : base(opCode)
    {
        if (opCode.OperandType == OperandType.InlineBrTarget)
        {
            this.Delta = delta;
            this.DeltaSize = sizeof(int);
        }
        else if (opCode.OperandType == OperandType.ShortInlineBrTarget)
        {
            Validate.InBounds(delta, sbyte.MinValue, (int)sbyte.MaxValue + 1)
                .ThrowIfError();
            this.Delta = (sbyte)delta;
            this.DeltaSize = sizeof(sbyte);
        }
        else
        {
            throw new ArgumentException(null, nameof(OpCode));
        }

        this.TargetOffset = Offset + Delta + DeltaSize + 1;
    }

    public OpCodeBranchInstruction(OpCode opCode, ILLabel label)
        : base(opCode)
    {
        if (opCode.OperandType == OperandType.InlineBrTarget)
        {
            this.DeltaSize = sizeof(int);
        }
        else if (opCode.OperandType == OperandType.ShortInlineBrTarget)
        {
            this.DeltaSize = sizeof(sbyte);
        }
        else
        {
            throw new ArgumentException(null, nameof(OpCode));
        }
        
        this.Label = label;
        this.TargetOffset = label.Offset;
        this.Delta = TargetOffset - Offset - DeltaSize - 1;
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(base.RenderTo!)
            .If(Validate.IsNotNull(Label),
                static (tb, lbl) => tb.Render(lbl),
                (tb, _) => tb.Render(TargetOffset));
    }
}