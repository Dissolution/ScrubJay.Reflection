namespace ScrubJay.Reflection.IL.Emission;

public sealed class Emitter : EmitterBase<Emitter>, ISimpleEmitter<Emitter>
{
    public CleanEmitter Clean => new(this);
    
    public Emitter(DynamicILMethod method, ILGenerator ilGenerator) 
        : base(method, ilGenerator)
    {
    }
    
    
}