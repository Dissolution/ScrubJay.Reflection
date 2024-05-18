namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

public interface IArrayBuilder<out TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    TEmitter LoadLength();
    TEmitter LoadItem(Type itemType, bool asAddress = false);
    TEmitter LoadItem<TItem>(bool asAddress = false);
    
    TEmitter StoreItem(Type itemType);
    TEmitter StoreItem<TItem>();

    TEmitter New(Type itemType);
    TEmitter New<TItem>();
}

internal class ArrayBuilder<TEmitter> : BuilderBase<TEmitter>, IArrayBuilder<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    public ArrayBuilder(Emitter<TEmitter> emitter) : base(emitter)
    {
        
    }

    public TEmitter LoadLength()
    {
        return _emitter.Ldlen();
    }
    public TEmitter LoadItem(Type itemType, bool asAddress = false)
    {
        return asAddress ? _emitter.Ldelema(itemType) : _emitter.Ldelem(itemType);
    }
    public TEmitter LoadItem<TItem>(bool asAddress = false)
    {
        return asAddress ? _emitter.Ldelema<TItem>() : _emitter.Ldelem<TItem>();
    }
    public TEmitter StoreItem(Type itemType)
    {
        return _emitter.Stelem(itemType);
    }
    public TEmitter StoreItem<TItem>()
    {
        return _emitter.Stelem<TItem>();
    }
    public TEmitter New(Type itemType)
    {
        return _emitter.Newarr(itemType);
    }
    public TEmitter New<TItem>()
    {
        return _emitter.Newarr<TItem>();
    }
}