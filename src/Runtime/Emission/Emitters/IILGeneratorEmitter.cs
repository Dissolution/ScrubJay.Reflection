namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

public interface IILGeneratorEmitter<TEmitter> :
    IGeneratorEmitter<TEmitter>,
    IOpCodeEmitter<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    
}