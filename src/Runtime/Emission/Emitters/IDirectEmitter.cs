namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

public interface IDirectEmitter<TEmitter> : 
    IOpCodeEmitter<TEmitter>,
    IGeneratorEmitter<TEmitter>,
    IDirectOperationEmitter<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    ICleanEmitter CleanEmitter { get; }
    //ISimpleEmitter SimpleEmitter { get; }
}

public interface IDirectEmitter : IDirectEmitter<IDirectEmitter>;