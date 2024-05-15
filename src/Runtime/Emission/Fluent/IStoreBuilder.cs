namespace ScrubJay.Reflection.Runtime.Emission.Fluent;

public interface IStoreBuilder<TEmitter>
    where TEmitter : IFluentEmitter<TEmitter>
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