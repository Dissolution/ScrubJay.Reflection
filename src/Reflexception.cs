using System.Collections.Concurrent;
using ScrubJay.Collections;
using ScrubJay.Reflection.Runtime;
using ScrubJay.Reflection.Runtime.Naming;
using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection;

/// <summary>
/// An <see cref="Exception"/> thrown during Reflection operations
/// </summary>
public class Reflexception : Exception
{
    private static readonly Action<Exception, string?> _setExceptionMessage;
    private static readonly Action<Exception, Exception?> _setExceptionInnerException;

    static Reflexception()
    {
        var exceptionMessageField = Mirror.Search<Exception>()
            .TryFindMember<FieldInfo>(b => b
                .IsField
                .NonPublic
                .Instance
                .Name("_message")
                .ValueType<string>())
            .OkOrThrow();

        _setExceptionMessage = RuntimeBuilder.EmitDelegate<Action<Exception, string?>>(emitter => emitter
            .Ldarg(0)
            .Ldarg(1)
            .Stfld(exceptionMessageField)
            .Ret());

        var exceptionInnerExceptionField = Mirror.Search<Exception>()
            .TryFindMember<FieldInfo>(b => b
                .IsField
                .NonPublic
                .Instance
                .Name("_innerException")
                .ValueType<Exception>())
            .OkOrThrow();

        _setExceptionInnerException = RuntimeBuilder.EmitDelegate<Action<Exception, Exception?>>(emitter => emitter
            .Ldarg(0)
            .Ldarg(1)
            .Stfld(exceptionInnerExceptionField)
            .Ret());
    }

    private sealed record class ExceptionCtorKey(Type ExceptionType, Type[] CtorArgTypes)
    {
        public static ExceptionCtorKey Create(Type exType, params Type[] argTypes) => new(exType, argTypes);
        
        public static ExceptionCtorKey Create<TException>(params Type[] argTypes)
            where TException : Exception
            => new(typeof(TException), argTypes);
    }

    private static readonly ConcurrentDictionary<ExceptionCtorKey, Delegate> _exceptionCtorCache = new();
    
    public static TException Create<TException>()
        where TException : Exception, new()
        => Activator.CreateInstance<TException>();

    private static TDel CreateCtr<TDel>(ExceptionCtorKey key)
        where TDel : Delegate
    {
        var ctor = Mirror.Search(key.ExceptionType)
            .TryFindMember(b => b.IsConstructor.Instance.Parameters(key.CtorArgTypes))
            .OkOrThrow();
        var del = RuntimeBuilder.EmitDelegate<TDel>(emitter => emitter.LoadArgs(..key.CtorArgTypes.Length).Newobj(ctor).Ret());
        return del;
    }
    
    // public static TException Create<TException>(ref InterpolateDeeper message)
    //     where TException : Exception
    // {
    //     string msg = message.ToStringAndDispose();
    //     var construct = _exceptionCtorCache.GetOrAdd<ExceptionCtorKey, Func<string, TException>>(
    //         ExceptionCtorKey.Create<TException>(typeof(string)),
    //         CreateCtr<Func<string, TException>>);
    //     return construct(msg);
    // }
    //
    // public static TException Create<TException>(ref InterpolateDeeper message, string? paramName)
    //     where TException : Exception
    // {
    //     string msg = message.ToStringAndDispose();
    //     var construct = _exceptionCtorCache.GetOrAdd<ExceptionCtorKey, Func<string, string?, TException>>(
    //         ExceptionCtorKey.Create<TException>(typeof(string), typeof(string)),
    //         static key => CreateCtr<Func<string, string?, TException>>(key));
    //     return construct(msg, paramName);
    // }
    
    
    

    public new string Message
    {
        get => base.Message;
        init => _setExceptionMessage(this, value);
    }

    public new Exception? InnerException
    {
        get => base.InnerException;
        init => _setExceptionInnerException(this, value);
    }

    private DictionaryAdapter<string, object?>? _data = null;

    public new IDictionary<string, object?> Data => _data ??= new(base.Data);

    public Reflexception() : base(message: null) { }

    public Reflexception(string? message) : base(message) { }

    // public Reflexception(ref InterpolateDeeper message) : base(message.ToStringAndDispose()) { }

    public override string ToString()
    {
        return base.ToString();
    }
}