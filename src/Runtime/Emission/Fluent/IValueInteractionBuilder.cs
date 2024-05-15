namespace ScrubJay.Reflection.Runtime.Emission.Fluent;

public interface IValueInteractionBuilder<TEmitter>
    where TEmitter : IFluentEmitter<TEmitter>
{
    TEmitter Box(Type type, bool ifNeeded = false);
    TEmitter Box<T>(bool ifNeeded = false);
    TEmitter Unbox(Type type, bool asAddress, bool ifNeeded = false);
    TEmitter Unbox<T>(bool asAddress, bool ifNeeded = false);
    TEmitter CastClass(Type classType);
    TEmitter CastClass<TClass>() where TClass : class;
    TEmitter IsInstance(Type instanceType);
    TEmitter IsInstance<TInstance>();
}