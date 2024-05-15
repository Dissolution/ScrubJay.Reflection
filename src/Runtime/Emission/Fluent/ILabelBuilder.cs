namespace ScrubJay.Reflection.Runtime.Emission.Fluent;

public interface ILabelBuilder<TEmitter>
    where TEmitter : IFluentEmitter<TEmitter>
{
    TEmitter Define(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);
    TEmitter Mark(EmitterLabel label);
    TEmitter DefineAndMark(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);

}