namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

public interface ILabelBuilder<out TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    TEmitter Define(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);
    TEmitter Mark(EmitterLabel label);
    TEmitter DefineAndMark(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);
}

internal class LabelBuilder<TEmitter> : BuilderBase<TEmitter>, ILabelBuilder<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    public LabelBuilder(Emitter<TEmitter> emitter) : base(emitter)
    {
    }
    public TEmitter Define(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null)
        => _emitter.DefineLabel(out label, labelName);
    public TEmitter Mark(EmitterLabel label)
        => _emitter.MarkLabel(label);
    public TEmitter DefineAndMark(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null)
        => Define(out label, labelName).MarkLabel(label);
}