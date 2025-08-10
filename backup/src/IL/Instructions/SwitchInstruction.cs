namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class SwitchInstruction : OpCodeInstruction
{
    private int[]? _deltas;
    private ILLabel[]? _labels;
    
    public ILOffset[]? TargetOffsets { get; private set; }
    
    public int Count { get; }

    internal protected override void Fixup(ILOffset instructionOffset)
    {
        var targetOffset = instructionOffset + (1 + sizeof(int) + (sizeof(int) * cases));

        
        if (_deltas is not null)
        {
            TargetOffsets = new ILOffset[Count];
             for (int i = 0; i < Count; i++)
            {
                TargetOffsets[i] = targetOffset + _deltas[i];
            }
        }
        else if (_labels is not null)
        {
            TargetOffsets = new ILOffset[Count];
            for (int i = 0; i < Count; i++)
            {
                TargetOffsets[i] = targetOffset + _labels[i].Offset;
            }
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    public override int Size => OpCode.Size + ((1 + Count) * 4);

    public SwitchInstruction(OpCode opCode, int[] deltas) : base(opCode)
    {
        if (opCode != OpCodes.Switch)
            throw new ArgumentException(null, nameof(opCode));
        _deltas = deltas;
    }
    
    public SwitchInstruction(OpCode opCode, int[] deltas) : base(opCode)
    {
        if (opCode != OpCodes.Switch)
            throw new ArgumentException(null, nameof(opCode));
        _deltas = deltas;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(b => base.RenderTo(b))
            .Append('[')
            .EnumerateAndDelimit(TargetOffsets, static (t, off) => off.RenderTo(t), ", ")
            .Append(']');
    }
}