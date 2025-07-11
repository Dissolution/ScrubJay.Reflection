using ScrubJay.Reflection.IL.LabelOffSetManagement;

namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public class OpCodeLabelInstruction : OpCodeBranchInstruction
{
    public override int Delta => (int)(TargetOffset - Offset - DeltaTypeSize - 1);

    public override ILOffset TargetOffset => Label.Offset;

    public ILLabel Label { get; }

    public OpCodeLabelInstruction(OpCode opCode, ILLabel label)
        : base(opCode)
    {
        this.Label = label;
    }
    
    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .Render(Label);
    }
}




[PublicAPI]
public class OpCodeBranchInstruction : OpCodeInstruction
{
    public virtual int Delta { get; }
    
    public int DeltaTypeSize { get; }
    
    public virtual ILOffset TargetOffset =>  Offset + Delta + DeltaTypeSize + 1;

    public override sealed int Size => OpCode.Size + DeltaTypeSize;

    protected OpCodeBranchInstruction(OpCode opCode)
        : base(opCode)
    {
        this.DeltaTypeSize = opCode.OperandType switch
        {
            OperandType.InlineBrTarget => sizeof(int),
            OperandType.ShortInlineBrTarget => sizeof(sbyte),
            _ => throw new ArgumentException(null, nameof(OpCode)),
        };
    }
    
    public OpCodeBranchInstruction(OpCode opCode, int delta)
        : base(opCode)
    {
        switch (opCode.OperandType)
        {
            case OperandType.InlineBrTarget:
                this.Delta = delta;
                this.DeltaTypeSize = sizeof(int);
                break;
            case OperandType.ShortInlineBrTarget:
                Validate.InBounds(delta, sbyte.MinValue, (int)sbyte.MaxValue + 1)
                    .ThrowIfError();
                this.Delta = (sbyte)delta;
                this.DeltaTypeSize = sizeof(sbyte);
                break;
            default:
                throw new ArgumentException(null, nameof(OpCode));
        }
    }


    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .Render(TargetOffset)
            .Append(':');
    }
}