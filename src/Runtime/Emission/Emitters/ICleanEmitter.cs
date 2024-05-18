namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

public interface ICleanEmitter<TEmitter> :
    ICleanOperationEmitter<TEmitter>,
    IGeneratorEmitter<TEmitter>,
    IILEmitter<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    IDirectEmitter DirectEmitter { get; }
    ISimpleEmitter SimpleEmitter { get; }
}

public interface ICleanEmitter : ICleanEmitter<ICleanEmitter>;