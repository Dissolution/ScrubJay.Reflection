using ScrubJay.Reflection.IL.LabelOffSetManagement;

namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class OpCodeSwitchInstruction : OpCodeInstruction
{
    private int[]? _deltas = null;
    private ILOffset[]? _targetOffsets = null;

    public int[] Deltas
    {
        get => _deltas ?? throw new NotImplementedException();
        set
        {
            _deltas = value;
            int cases = _deltas.Length;
            var targets = new ILOffset[cases];
            var targetOffset = Offset + (1 + sizeof(int) + (sizeof(int) * cases));
            for (int i = 0; i < cases; i++)
            {
                targets[i] = targetOffset + Deltas[i];
            }
            _targetOffsets = targets;
        }
    }

    public ILOffset[] TargetOffsets
    {
        get
        {
            if (_targetOffsets is null)
            {
                int cases = _deltas!.Length;
                var targets = new ILOffset[cases];
                var targetOffset = Offset + (1 + sizeof(int) + (sizeof(int) * cases));
                for (int i = 0; i < cases; i++)
                {
                    targets[i] = targetOffset + Deltas[i];
                }

                _targetOffsets = targets;
            }
            return _targetOffsets;
        }
        set => throw new NotImplementedException();
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

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(b => base.RenderTo(b))
            .Append('[')
            .EnumerateAndDelimit(TargetOffsets, static (t, off) => off.RenderTo(t), ", ")
            .Append(']');
    }
}