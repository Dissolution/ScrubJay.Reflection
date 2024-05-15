using ScrubJay.Reflection.Runtime.Emission.Fluent;

namespace ScrubJay.Reflection.Runtime.Emission;

public interface IBasicEmitter<TEmitter> : 
    IILEmitter<TEmitter>,
    IOpCodeEmitter<TEmitter>,
    IGeneratorEmitter<TEmitter>,
    IOperationEmitter<TEmitter>
    where TEmitter : IBasicEmitter<TEmitter>
{
    IFluentEmitter Fluent { get; }
}

public interface IBasicEmitter : IBasicEmitter<IBasicEmitter>;