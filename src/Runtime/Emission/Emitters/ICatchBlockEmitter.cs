namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

public interface ICatchBlockEmitter<TEmitter> : ISimpleEmitter<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    TEmitter ReThrow();

    TEmitter Leave(EmitterLabel label);
    TEmitter Leave(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string labelName = "");
}