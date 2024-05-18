namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

public interface ILabelBuilder<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    TEmitter Define(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);
    TEmitter Mark(EmitterLabel label);
    TEmitter DefineAndMark(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);

}