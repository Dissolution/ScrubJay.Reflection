namespace ScrubJay.Reflection.OLD.OLD.Runtime.Emission.Emitters;

public interface IILGeneratorEmitter<TEmitter> :
    IGeneratorEmitter<TEmitter>,
    IOpCodeEmitter<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    
}