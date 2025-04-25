using ScrubJay.Reflection.IL.LabelOffSetManagement;

namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class OpCodeSwitchInstruction : OpCodeInstruction
{
    public int[] Deltas { get; }

    public ILOffset[] TargetOffsets
    {
        get
        {
            int cases = Deltas.Length;

            var targets = new ILOffset[cases];

            var targetOffset = Offset + (1 + sizeof(int) + (sizeof(int) * cases));
        
            for (int i = 0; i < cases; i++)
            {
                targets[i] = targetOffset + Deltas[i];
            }
            
            return targets;
        }
    }
    
    public override int Size
    {
        get
        {
            int size = OpCode.Size;
            size += ((1 + Deltas.Length) * 4);
            return size;
        }
    }
    
    public OpCodeSwitchInstruction(OpCode opCode, int[] deltas) : base(opCode)
    {
        if (opCode != OpCodes.Switch)
            throw new ArgumentException(null, nameof(opCode));
        this.Deltas = deltas;
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(b => base.RenderTo(b))
            .Append('[')
            .Delimit(", ", TargetOffsets, static (t, off) => off.RenderTo(t))
            .Append(']');
    }
}