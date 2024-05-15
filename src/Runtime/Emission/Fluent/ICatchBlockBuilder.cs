namespace ScrubJay.Reflection.Runtime.Emission.Fluent;

public interface ICatchBlockBuilder<TEmitter> : IFluentEmitter<TEmitter>
    where TEmitter : IFluentEmitter<TEmitter>
{
    TEmitter ReThrow();

    TEmitter Leave(EmitterLabel label);
    TEmitter Leave(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string labelName = "");
}