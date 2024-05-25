//namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;
//
//public interface IValueInteractionBuilder<out TEmitter>
//    where TEmitter : IILEmitter<TEmitter>
//{
//    TEmitter Box(Type type, bool ifNeeded = false);
//    TEmitter Box<T>(bool ifNeeded = false);
//    
//    TEmitter Unbox(Type type, bool asAddress, bool ifNeeded = false);
//    TEmitter Unbox<T>(bool asAddress, bool ifNeeded = false);
//    
//    TEmitter CastClass(Type classType);
//    TEmitter CastClass<TClass>() where TClass : class;
//    
//    TEmitter IsInstance(Type instanceType);
//    TEmitter IsInstance<TInstance>();
//    
//    TEmitter SizeOf(Type type);
//    TEmitter SizeOf<T>() where T : unmanaged;
//}
//
//internal class ValueInteractionBuilder<TEmitter> : BuilderBase<TEmitter>, IValueInteractionBuilder<TEmitter>
//    where TEmitter : IILEmitter<TEmitter>
//{
//    public ValueInteractionBuilder(Emitter<TEmitter> emitter) : base(emitter)
//    {
//    }
//    public TEmitter Box(Type type, bool ifNeeded = false)
//    {
//        if (!ifNeeded || type.IsValueType)
//            return _emitter.Box(type);
//        return _emitter;
//    }
//    public TEmitter Box<T>(bool ifNeeded = false)
//    {
//        return _emitter.If(!ifNeeded || typeof(T).IsValueType, e => e.Box<T>());
//    }
//    public TEmitter Unbox(Type type, bool asAddress, bool ifNeeded = false)
//    {
//        return _emitter.If(!ifNeeded || type.IsValueType,
//            e => e.If(asAddress, a => a.Unbox(type), v => v.Unbox_Any(type)));
//    }
//    public TEmitter Unbox<T>(bool asAddress, bool ifNeeded = false)
//    {
//        return _emitter.If(!ifNeeded || typeof(T).IsValueType,
//            e => e.If(asAddress, a => a.Unbox<T>(), v => v.Unbox_Any<T>()));
//    }
//    public TEmitter CastClass(Type classType)
//    {
//        return _emitter.Castclass(classType);
//    }
//    public TEmitter CastClass<TClass>() where TClass : class
//    {
//        return _emitter.Castclass<TClass>();
//    }
//    public TEmitter IsInstance(Type instanceType)
//    {
//        return _emitter.Isinst(instanceType);
//    }
//    public TEmitter IsInstance<TInstance>()
//    {
//        return _emitter.Isinst<TInstance>();
//    }
//    public TEmitter SizeOf(Type type)
//    {
//        return _emitter.Sizeof(type);
//    }
//    public TEmitter SizeOf<T>() where T : unmanaged
//    {
//        return _emitter.Sizeof<T>();
//    }
//}