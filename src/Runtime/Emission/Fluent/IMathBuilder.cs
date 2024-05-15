namespace ScrubJay.Reflection.Runtime.Emission.Fluent;

public interface IMathBuilder<TEmitter>
    where TEmitter : IFluentEmitter<TEmitter>
{
    IConvertBuilder<TEmitter> ThrowOnOverflow { get; }
    IConvertBuilder<TEmitter> Unsigned { get; }

    TEmitter Add();
    TEmitter Divide();
    TEmitter Multiply();
    TEmitter DivisionRemainder();
    TEmitter Subtract();
}