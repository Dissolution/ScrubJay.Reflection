namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

internal abstract class BuilderBase<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    protected readonly Emitter<TEmitter> _emitter;

    protected ICleanEmitter CleanEmitter => (ICleanEmitter)(ICleanEmitter<ICleanEmitter>)_emitter;
    
    protected BuilderBase(Emitter<TEmitter> emitter)
    {
        _emitter = emitter;
    }
}