namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

public interface ILoadBuilder<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    ILoadBuilder<TEmitter> AsAddress { get; }

    TEmitter Argument(int index);
    TEmitter Argument(ParameterInfo parameter);

    TEmitter Local(EmitterLocal local);

    TEmitter Field(FieldInfo field);
    
    TEmitter Token(Type type);
    TEmitter Token(FieldInfo field);
    TEmitter Token(MethodInfo method);

    TEmitter ArrayLength();
    TEmitter ArrayItem(Type itemType);
    TEmitter ArrayItem<TItem>();

    TEmitter FromAddress(Type type);
    TEmitter FromAddress<T>();
}