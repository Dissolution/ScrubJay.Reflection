namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

public interface ILocalBuilder<out TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    TEmitter Declare(Type localType, out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);
    TEmitter Declare<T>(out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);
    TEmitter Load(EmitterLocal local, bool asAddress = false);
    TEmitter Store(EmitterLocal local);
}

internal class LocalBuilder<TEmitter> : BuilderBase<TEmitter>, ILocalBuilder<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    public LocalBuilder(Emitter<TEmitter> emitter) : base(emitter)
    {
    }
    public TEmitter Declare(Type localType, out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null)
    {
        return _emitter.DeclareLocal(localType, out local, localName);
    }
    
    public TEmitter Declare<T>(out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null)
    {
        return _emitter.DeclareLocal<T>(out local, localName);
    }
    
    public TEmitter Load(EmitterLocal local, bool asAddress = false)
    {
        return asAddress ? _emitter.Ldloca(local) : _emitter.Ldloc(local);
    }
    
    public TEmitter Store(EmitterLocal local)
    {
        return _emitter.Stloc(local);
    }
}