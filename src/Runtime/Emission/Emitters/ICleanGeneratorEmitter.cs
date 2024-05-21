using ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

public interface ICleanGeneratorEmitter<TEmitter> : IILEmitter<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    ITryCatchFinallyBuilder<TEmitter> Try(Action<TEmitter> emitTryBlock);
    
    TEmitter Scoped(Action<TEmitter> emitScopedBlock);
    
    TEmitter Declare(Type localType, out EmitterLocal local, bool pinned = false, [CallerArgumentExpression(nameof(local))] string? localName = null);
    TEmitter Declare<T>(out EmitterLocal local, bool pinned = false, [CallerArgumentExpression(nameof(local))] string? localName = null);
   
    TEmitter Define(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);
    TEmitter Mark(EmitterLabel label);
    TEmitter DefineAndMark(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);
}