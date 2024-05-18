namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

public interface IMathBuilder<TEmitter>
    where TEmitter : ISimpleEmitter<TEmitter>
{
    IConvertBuilder<TEmitter> ThrowOnOverflow { get; }
    IConvertBuilder<TEmitter> Unsigned { get; }

    TEmitter Add();
    TEmitter Divide();
    TEmitter Multiply();
    TEmitter DivisionRemainder();
    TEmitter Subtract();
}