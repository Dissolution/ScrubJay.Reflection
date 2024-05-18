namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

public interface IBranchBuilder<out TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    IBranchBuilder<TEmitter> Unsigned { get; }

    TEmitter To(EmitterLabel label);
    TEmitter To(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);

    TEmitter If(bool boolean, EmitterLabel label);
    TEmitter If(bool boolean, out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);

    TEmitter If(CompareOp op, EmitterLabel label);
    TEmitter If(CompareOp op, out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);
}

internal class BranchBuilder<TEmitter> : BuilderBase<TEmitter>, IBranchBuilder<TEmitter>
    where TEmitter : ICleanEmitter<TEmitter>
{
    private bool _unsigned = false;

    public BranchBuilder(Emitter<TEmitter> emitter) : base(emitter)
    {

    }

    public IBranchBuilder<TEmitter> Unsigned
    {
        get
        {
            _unsigned = true;
            return this;
        }
    }

    public TEmitter To(EmitterLabel label)
    {
        return _emitter.Br(label);
    }
    public TEmitter To(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null)
    {
        return _emitter
            .DefineLabel(out label, labelName)
            .Br(label);
    }
    public TEmitter If(bool boolean, EmitterLabel label)
    {
        return boolean ? _emitter.Brtrue(label) : _emitter.Brfalse(label);
    }
    public TEmitter If(bool boolean, out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null)
    {
        _emitter.DefineLabel(out label, labelName);
        return If(boolean, label);

    }
    public TEmitter If(CompareOp op, EmitterLabel label)
    {
        return op switch
        {
            CompareOp.NotEqual => _emitter.Bne(label),
            CompareOp.Equal => _emitter.Beq(label),
            CompareOp.GreaterThan => _emitter.Bgt(label, _unsigned),
            CompareOp.LessThan => _emitter.Blt(label, _unsigned),
            CompareOp.GreaterThanOrEqual => _emitter.Bge(label, _unsigned),
            CompareOp.LessThanOrEqual => _emitter.Ble(label, _unsigned),
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null),
        };
    }
    public TEmitter If(CompareOp op, out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null)
    {
        _emitter.DefineLabel(out label, labelName);
        return If(op, label);
    }
}