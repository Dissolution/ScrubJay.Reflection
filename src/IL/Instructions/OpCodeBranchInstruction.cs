namespace ScrubJay.Reflection.IL.Instructions;

public sealed class OpCodeBranchInstruction : OpCodeInstruction
{
    public int Delta { get; }

    public ILOffset TargetOffset => Offset + Delta + 1 + (IsShort ? sizeof(sbyte) : sizeof(int));

    public override int Size => OpCode.Size + (IsShort ? sizeof(sbyte) : sizeof(int));
    
    internal OpCodeBranchInstruction(OpCode opCode, int delta)
        : base(opCode)
    {
        this.Delta = delta;
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(base.RenderTo!)
            .Invoke(TargetOffset.RenderTo!);
    }
}