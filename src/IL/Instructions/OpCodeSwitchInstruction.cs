namespace ScrubJay.Reflection.IL.Instructions;

public sealed class OpCodeSwitchInstruction : OpCodeInstruction
{
    public int[] Deltas { get; }
    
    private int[]? _targetOffsets;
    public int[] TargetOffsets
    {
        get
        {
            if (_targetOffsets is null)
            {
                int cases = Deltas.Length;
                int itself = 1 + sizeof(int) + (sizeof(int) * cases);
                _targetOffsets = new int[cases];
                for (int i = 0; i < cases; i++)
                {
                    _targetOffsets[i] = Offset + Deltas[i] + itself;
                }
            }
            return _targetOffsets;
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
    
    public OpCodeSwitchInstruction(int[] deltas) : base(OpCodes.Switch)
    {
        this.Deltas = deltas;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo)
            .If(Validate.IsNotNull(_targetOffsets),
                static (tb, offsets) => tb.Delimit(", ", offsets, static (t, off) => t.Append($"IL_{off:X4}")),
                (tb, _) => tb.Delimit(", ", Deltas, static (tb, d) => tb.Append('Δ').Render(d)));
    }
}