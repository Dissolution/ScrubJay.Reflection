namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

public interface ICleanEmitter<TEmitter> :
    ICleanOperationEmitter<TEmitter>,
    IGeneratorEmitter<TEmitter>,
    IILEmitter<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    IDirectEmitter DirectEmitter { get; }
    ISimpleEmitter SimpleEmitter { get; }

    TEmitter If(bool predicate, Action<TEmitter>? emitIfTrue, Action<TEmitter>? emitIfFalse = null);
    TEmitter IfNotNull<T>(T? value, Action<TEmitter, T> emitIfNotNull);
}

public interface ICleanEmitter : ICleanEmitter<ICleanEmitter>;