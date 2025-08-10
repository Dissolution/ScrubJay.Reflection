using ScrubJay.Reflection.Validation;

namespace ScrubJay.Reflection.IL.Emission;

public sealed class TryCatchFinally<E> : TryCatchFinallyBuilder<ITryCatchFinally<E>, E>,
    ITryCatchFinally<E> 
    where E : IGenEmitter<E>, IOperationEmitter<E>
{
    internal TryCatchFinally(E emitter) : base(emitter)
    {
        
    }
}


public interface ITryCatchFinally<E> : ITryCatchFinallyBuilder<ITryCatchFinally<E>, E>;

public interface ITryCatchFinallyBuilder<B, E> : IFluentBuilder<B>
    where B : ITryCatchFinallyBuilder<B, E>
{
    B Try(Action<E, ILLabel> emitTryBlock);

    B Catch<X>(Action<E, ILLabel> emitCatchBlock)
        where X : Exception;

    B Catch(Type exceptionType, Action<E, ILLabel> emitCatchBlock);

    B Swallow<X>()
        where X : Exception;

    B Swallow(Type exceptionType);
    
    E Finally();

    E Finally(Action<E, ILLabel> emitFinallyBlock);
}

public abstract class TryCatchFinallyBuilder<B, E> : IFluentBuilder<B>,
    ITryCatchFinallyBuilder<B,E> 
    where B : ITryCatchFinallyBuilder<B, E>
    where E : IGenEmitter<E>, IOperationEmitter<E>
{
    private readonly B _builder;
    private readonly E _emitter;
    private readonly ILLabel _endLabel;

    B IFluentBuilder<B>.Self => _builder;

    public ILLabel EndLabel => _endLabel;

    protected TryCatchFinallyBuilder(E emitter)
    {
        _builder = (B)(ITryCatchFinallyBuilder<B,E>)this;
        _emitter = emitter;
        _emitter.BeginExceptionBlock(out _endLabel);
    }

    public B Try(Action<E, ILLabel> emitTryBlock)
    {
        Throw.IfNull(emitTryBlock);
        emitTryBlock(_emitter, EndLabel);
        return _builder;
    }
    
    public B Catch<X>(Action<E, ILLabel> emitCatchBlock)
        where X : Exception
    {
        Throw.IfNull(emitCatchBlock);
        var emitter = _emitter.BeginCatchBlock<X>();
        emitCatchBlock(emitter, EndLabel);
        return _builder;
    }
    
    public B Catch(Type exceptionType, Action<E, ILLabel> emitCatchBlock)
    {
        MemberAssert.IsExceptionType(exceptionType);
        Throw.IfNull(emitCatchBlock);
        var emitter = _emitter.BeginCatchBlock(exceptionType);
        emitCatchBlock(emitter, EndLabel);
        return _builder;
    }

    public B Swallow<X>()
        where X : Exception
    {
        return Catch<X>(static (emitter, end) => emitter.Pop().Leave(end));
    }

    public B Swallow(Type exceptionType)
    {
        MemberAssert.IsExceptionType(exceptionType);
        return Catch(exceptionType, static (emitter, end) => emitter.Pop().Leave(end));
    }

    /// <summary>
    /// Ends this <c>try/catch</c> block
    /// </summary>
    public E Finally()
    {
        return _emitter.EndExceptionBlock();
    }

    public E Finally(Action<E, ILLabel> emitFinallyBlock)
    {
        var emitter = _emitter.BeginFinallyBlock();
        emitFinallyBlock(emitter, EndLabel);
        return _emitter.EndExceptionBlock();
    }
}
