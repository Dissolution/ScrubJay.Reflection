namespace ScrubJay.Reflection.Runtime.Emission.Fluent;

public interface IBitwiseBuilder<TEmitter>
    where TEmitter : IFluentEmitter<TEmitter>
{
    TEmitter And();
    TEmitter Negate();
    TEmitter Not();
    TEmitter Or();
    TEmitter LeftShift();
    TEmitter RightShift(bool unsigned = false);
    TEmitter Xor();
}