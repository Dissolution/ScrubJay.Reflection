namespace ScrubJay.Reflection.IL.Instructions;

public sealed class OpCodeSwitchInstruction : OpCodeInstruction
{
    public int[] Deltas { get; }
    
    public ILOffset[] TargetOffsets { get; }
    
    public override int Size
    {
        get
        {
            int size = OpCode.Size;
            size += ((1 + Deltas.Length) * 4);
            return size;
        }
    }
    
    public OpCodeSwitchInstruction(int[] deltas) : base(OpCodes.Switch)
    {
        int cases = deltas.Length;
        int itself = 1 + sizeof(int) + (sizeof(int) * cases);
        var targets = new ILOffset[cases];
        for (int i = 0; i < cases; i++)
        {
            targets[i] = new(Offset + deltas[i] + itself);
        }

        this.Deltas = deltas;
        this.TargetOffsets = targets;
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(b => base.RenderTo(b))
            .Append('[')
            .Delimit(", ", TargetOffsets, static (t, off) => off.RenderTo(t))
            .Append(']');
    }
}