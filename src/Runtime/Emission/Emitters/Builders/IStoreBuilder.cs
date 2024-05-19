namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

public interface IStoreBuilder<out TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    TEmitter Argument(int index);
    TEmitter Argument(ParameterInfo parameter);

    TEmitter Local(EmitterLocal local);

    TEmitter Field(FieldInfo field);
    
    TEmitter ArrayItem(Type itemType);
    TEmitter ArrayItem<TItem>();

    TEmitter ToAddress(Type type);
    TEmitter ToAddress<T>();
}

internal class StoreBuilder<TEmitter> : BuilderBase<TEmitter>, IStoreBuilder<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    public StoreBuilder(Emitter<TEmitter> emitter) : base(emitter)
    {
    }
    public TEmitter Argument(int index) => _emitter.Starg(index);
    public TEmitter Argument(ParameterInfo parameter) => _emitter.Starg(parameter.Position);
    public TEmitter Local(EmitterLocal local) => _emitter.Stloc(local);
    public TEmitter Field(FieldInfo field) => _emitter.Stfld(field);
    public TEmitter ArrayItem(Type itemType) => _emitter.Stelem(itemType);
    public TEmitter ArrayItem<TItem>() => _emitter.Stelem<TItem>();
    public TEmitter ToAddress(Type type) => _emitter.Stind(type);
    public TEmitter ToAddress<T>() => _emitter.Stind<T>();
}