namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

public interface IBitwiseBuilder<out TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    TEmitter And();
    TEmitter Negate();
    TEmitter Not();
    TEmitter Or();
    TEmitter LeftShift();
    TEmitter RightShift(bool unsigned = false);
    TEmitter Xor();
}

internal class BitwiseBuilder<TEmitter> : BuilderBase<TEmitter>, IBitwiseBuilder<TEmitter>
    where TEmitter : IDirectOperationEmitter<TEmitter>
{
    public BitwiseBuilder(Emitter<TEmitter> emitter) : base(emitter)
    {
    }

    public TEmitter And() => _emitter.And();
    public TEmitter Negate() => _emitter.Neg();
    public TEmitter Not() => _emitter.Not();
    public TEmitter Or() => _emitter.Or();
    public TEmitter LeftShift() => _emitter.Shl();
    public TEmitter RightShift(bool unsigned = false) => unsigned ? _emitter.Shr_Un() : _emitter.Shr();
    public TEmitter Xor() => _emitter.Xor();
}