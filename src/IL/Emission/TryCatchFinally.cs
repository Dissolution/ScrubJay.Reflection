using ScrubJay.Reflection.IL.LabelOffSetManagement;
using ScrubJay.Reflection.Validation;

namespace ScrubJay.Reflection.IL.Emission;

public sealed class TryCatchFinally<E> : TryCatchFinallyBuilder<TryCatchFinally<E>, E>
    where E : IGenEmitter<E>
{
    internal TryCatchFinally(E emitter) : base(emitter)
    {
        
    }
}

public abstract class TryCatchFinallyBuilder<B, E> : BuilderBase<B>
    where B : TryCatchFinallyBuilder<B, E>
    where E : IGenEmitter<E>
{
    private readonly E _emitter;
    private readonly ILLabel _endLabel;

    public ILLabel EndLabel => _endLabel;

    protected TryCatchFinallyBuilder(E emitter)
        : base()
    {
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
