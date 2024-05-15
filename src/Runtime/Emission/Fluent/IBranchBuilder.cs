namespace ScrubJay.Reflection.Runtime.Emission.Fluent;

public interface IBranchBuilder<TEmitter>
    where TEmitter : IFluentEmitter<TEmitter>
{
    IBranchBuilder<TEmitter> Unsigned { get; }

    TEmitter To(EmitterLabel label);
    TEmitter To(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);

    TEmitter If(bool boolean, EmitterLabel label);
    TEmitter If(bool boolean, out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);
    
    TEmitter If(CompareOp op, EmitterLabel label);
    TEmitter If(CompareOp op, out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);

}