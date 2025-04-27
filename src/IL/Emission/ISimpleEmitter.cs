namespace ScrubJay.Reflection.IL.Emission;

[PublicAPI]
public interface ISimpleEmitter<E> : IGenEmitter<E>, IOperationEmitter<E>, IEmitter<E>
    where E : ISimpleEmitter<E>;

public sealed class Emitter : EmitterBase<Emitter>, ISimpleEmitter<Emitter>
{
    public CleanEmitter Clean => new(this);
    
    public Emitter(DynamicILMethod method, ILGenerator ilGenerator) 
        : base(method, ilGenerator)
    {
    }
    
    
}