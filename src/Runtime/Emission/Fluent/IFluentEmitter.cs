namespace ScrubJay.Reflection.Runtime.Emission.Fluent;

public interface IFluentEmitter<TEmitter> : IILEmitter<TEmitter>
    where TEmitter : IFluentEmitter<TEmitter>
{
    IBasicEmitter Basic { get; }
    
    ITryCatchFinallyBuilder<TEmitter> Try(Action<TEmitter> emitTryBlock);
    IBranchBuilder<TEmitter> Branch { get; }
    IValueInteractionBuilder<TEmitter> Value { get; }
    IConvertBuilder<TEmitter> Convert { get; }
    ICompareBuilder<TEmitter> Compare { get; }
    ILoadBuilder<TEmitter> Load { get; }
    IStoreBuilder<TEmitter> Store { get; }
    ILabelBuilder<TEmitter> Label { get; }
    IDebugBuilder<TEmitter> Debug { get; }
    
    TEmitter Scoped(Action<TEmitter> emitScopedBlock);
    
    TEmitter PushValue<T>(T? value);
    TEmitter Pop();
    
    TEmitter Call(MethodBase method);
    TEmitter SizeOf(Type type);
    TEmitter SizeOf<T>();
    
    TEmitter ThrowException(Type exceptionType, params object?[] args);
    TEmitter ThrowException<TException>(params object?[] args) where TException : Exception;
    
    TEmitter Return();
}

public interface IFluentEmitter : IFluentEmitter<IFluentEmitter>;


/* PushValue<Type>:
 * LdToken(type).Call(Type.GetTypeFromHandle()) 
 */


 
/*
internal abstract class FluentEmitter<TEmitter> : IFluentEmitter<TEmitter>
    where TEmitter : FluentEmitter<TEmitter>
{
    protected readonly TEmitter _emitter;

    public InstructionStream Instructions { get; }

    public FluentEmitter()
    {
        _emitter = (TEmitter)this;
        this.Instructions = new();
    }

//    public TEmitter LoadValue<T>(T? value)
//    {
//        if (value is null)
//            return Ldnull();
//        if (value is bool boolean)
//            return boolean ? Ldc_I4_1() : Ldc_I4_0();
//        if (value is byte b)
//            return Ldc_I4(b);
//        if (value is sbyte sb)
//            return Ldc_I4(sb);
//        if (value is short s)
//            return Ldc_I4(s);
//        if (value is ushort us)
//            return Ldc_I4(us);
//        if (value is int i)
//            return Ldc_I4(i);
//        if (value is uint ui)
//            return Ldc_I8(ui);
//        if (value is long l)
//            return Ldc_I8(l);
//        if (value is ulong ul)
//            return Ldc_I8((long)ul);
//        if (value is float f)
//            return Ldc_R4(f);
//        if (value is double d)
//            return Ldc_R8(d);
//        if (value is string str)
//            return Ldstr(str);
//        if (value is Type type)
//            return LoadType(type);
//        if (value is EmitterLocal local)
//            return Ldloc(local);
//
//        throw new NotImplementedException();
//    }
    
    //
//    TEmitter LoadType(Type type)
//    {
//        ArgumentNullException.ThrowIfNull(type);
//        return Ldtoken(type).Call(MemberCache.Methods.Type_GetTypeFromHandle);
//    }
//    TEmitter LoadType<T>() => LoadType(typeof(T));
//
//
//    /// <summary>
//    /// Loads the default value of a <paramref name="type"/> onto the stack, exactly like default(Type)
//    /// </summary>
//    TEmitter LoadDefault(Type type)
//    {
//        // Value types require more code
//        if (type.IsValueType)
//        {
//            return DeclareLocal(type, out var defaultValue)
//                .Ldloca(defaultValue)
//                .Initobj(type)
//                .Ldloc(defaultValue);
//        }
//        // Anything else defaults to null
//        return Ldnull();
//    }
//    TEmitter LoadDefault<T>() => LoadDefault(typeof(T));
//
//    TEmitter LoadDefaultAddress(Type type)
//    {
//        // Value types require more code
//        if (type.IsValueType)
//        {
//            return DeclareLocal(type, out var defaultValue)
//                .Ldloca(defaultValue)
//                .Initobj(type)
//                .Ldloca(defaultValue);
//        }
//        // Anything else defaults to null
//        return Ldnulla();
//    }
//    
//    TEmitter LoadDefaultAddress<T>() => LoadDefaultAddress(typeof(T));
//
//
//    /// <summary>
//    /// Loads a <c>null</c> reference onto the stack
//    /// </summary>
//    TEmitter Ldnulla() => Ldc_I4_0().Conv_U();
//
}
*/