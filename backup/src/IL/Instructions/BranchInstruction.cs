namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class BranchInstruction : OpCodeInstruction
{
    public int DeltaTypeSize { get; }

    public override int Size => OpCode.Size + DeltaTypeSize;

    public int? Delta { get; private set; }
    public ILLabel? Label { get; private set; }
    public ILOffset TargetOffset { get; private set; }


    public BranchInstruction(OpCode opCode, int delta)
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
                throw InvalidEnumException.Create(opCode.OperandType);
        }
    }

    public BranchInstruction(OpCode opCode, ILLabel label)
        : base(opCode)
    {
        this.Label = label;
        this.TargetOffset = label.Offset;
        switch (opCode.OperandType)
        {
            case OperandType.InlineBrTarget:
                this.DeltaTypeSize = sizeof(int);
                break;
            case OperandType.ShortInlineBrTarget:
                this.DeltaTypeSize = sizeof(sbyte);
                break;
            default:
                throw InvalidEnumException.Create(opCode.OperandType);
        }
    }

    internal void SetOffset(ILOffset offset)
    {
        if (Delta.TryGetValue(out var delta))
        {
            TargetOffset = offset + delta + DeltaTypeSize + 1;
        }
        else if (Label.TryGetValue(out var label))
        {
            Delta = (int)(label.Offset - offset - DeltaTypeSize);
        }
    }

    internal protected override TextBuilder RenderArgs(TextBuilder builder)
    {
        if (Label.TryGetValue(out var label))
            return builder.Render(label);
        if (Delta.TryGetValue(out var delta))
            return builder.Append('Δ').Format(delta);
        if (!TargetOffset.IsUnknown)
            return builder.Render(TargetOffset);
        return builder.Append("???");
    }
}