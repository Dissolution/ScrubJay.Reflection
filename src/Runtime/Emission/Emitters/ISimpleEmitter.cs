using ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

public interface ISimpleEmitter<TEmitter> : IILEmitter<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    IDirectEmitter DirectEmitter { get; }
    ICleanEmitter CleanEmitter { get; }
    
    IArrayBuilder<TEmitter> Array { get; }
    IBitwiseBuilder<TEmitter> Bitwise { get; }
    IBranchBuilder<TEmitter> Branch { get; }
    IValueInteractionBuilder<TEmitter> Value { get; }
    IConvertBuilder<TEmitter> Convert { get; }
    ICompareBuilder<TEmitter> Compare { get; }
    ILoadBuilder<TEmitter> Load { get; }
    IStoreBuilder<TEmitter> Store { get; }
    ILabelBuilder<TEmitter> Label { get; }
    ILocalBuilder<TEmitter> Local { get; }
    IDebugBuilder<TEmitter> Debug { get; }
    IExceptionBuilder<TEmitter> Exception { get; }
    
    ITryCatchFinallyBuilder<TEmitter> Try(Action<TEmitter> emitTryBlock);
    TEmitter Scoped(Action<TEmitter> emitScopedBlock);
    
    TEmitter PushValue<T>(T? value);
    TEmitter Pop();
    
    TEmitter Call(MethodBase method);

    TEmitter Return();
}

public interface ISimpleEmitter : ISimpleEmitter<ISimpleEmitter>;