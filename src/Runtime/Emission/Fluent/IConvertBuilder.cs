namespace ScrubJay.Reflection.Runtime.Emission.Fluent;

public interface IConvertBuilder<TEmitter>
    where TEmitter : IFluentEmitter<TEmitter>
{
    IConvertBuilder<TEmitter> ThrowOnOverflow { get; }
    IConvertBuilder<TEmitter> Unsigned { get; }
    TEmitter To(Type destType);
    TEmitter To<T>();
}