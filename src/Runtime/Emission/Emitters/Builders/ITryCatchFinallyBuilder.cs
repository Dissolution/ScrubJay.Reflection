namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEmitter">The <see cref="IILEmitter"/> that this <c>try</c>/<c>catch</c> builder emits to</typeparam>
public interface ITryCatchFinallyBuilder<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    /// <summary>
    /// Gets the <see cref="EmitterLabel"/> that marks the end position of this <c>try/catch/finally</c> block<br/>
    /// May be <c>null</c> if the position has not yet been marked
    /// </summary>
    EmitterLabel? TryBlockEndLabel { get; }

    /*
    /// <summary>
    ///
    /// </summary>
    /// <param name="emitTryBlock"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="emitTryBlock"/> is <c>null</c></exception>
    ITryCatchFinallyBuilder<TEmitter> Try(Action<TEmitter> emitTryBlock);
    */

    /// <summary>
    /// 
    /// </summary>
    /// <param name="exceptionType"></param>
    /// <param name="emitCatchBlock"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="exceptionType"/> or <paramref name="emitCatchBlock"/> is <c>null</c></exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="exceptionType"/> is not a valid <see cref="Exception"/> <see cref="Type"/></exception>
    ITryCatchFinallyBuilder<TEmitter> Catch(Type exceptionType, Action<TEmitter> emitCatchBlock);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emitCatchBlock"></param>
    /// <typeparam name="TException"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="emitCatchBlock"/> is <c>null</c></exception>
    ITryCatchFinallyBuilder<TEmitter> Catch<TException>(Action<TEmitter> emitCatchBlock)
        where TException : Exception;

    /// <summary>
    /// End this <c>try/catch</c> block and return to standard emission
    /// </summary>
    /// <returns>
    /// The original <typeparamref name="TEmitter"/>
    /// </returns>
    TEmitter Finally();

    /// <summary>
    /// 
    /// </summary>
    /// <returns>
    /// The original <typeparamref name="TEmitter"/>
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="emitFinallyBlock"/> is <c>null</c></exception>
    TEmitter Finally(Action<TEmitter> emitFinallyBlock);
}

internal sealed class TryCatchFinallyBuilder<TEmitter> : BuilderBase<TEmitter>, ITryCatchFinallyBuilder<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    private EmitterLabel _endTryLabel;

    public EmitterLabel? TryBlockEndLabel => _endTryLabel;

    public TryCatchFinallyBuilder(Emitter<TEmitter> emitter) : base(emitter)
    {

    }

    public ITryCatchFinallyBuilder<TEmitter> Try(Action<TEmitter> tryBlock)
    {
        _emitter.BeginExceptionBlock(out _endTryLabel).Invoke(tryBlock);
        return this;
    }

    public ITryCatchFinallyBuilder<TEmitter> Catch(Type exceptionType, Action<TEmitter> catchBlock)
    {
        _emitter.BeginCatchBlock(exceptionType).Invoke(catchBlock);
        return this;
    }

    public ITryCatchFinallyBuilder<TEmitter> Catch<TException>(Action<TEmitter> emitCatchBlock)
        where TException : Exception
    {
        _emitter.BeginCatchBlock<TException>().Invoke(emitCatchBlock);
        return this;
    }

    public TEmitter Finally() => _emitter;

    public TEmitter Finally(Action<TEmitter>? finallyBlock)
    {
        return _emitter.IfNotNull(finallyBlock, (e, f) => e
                .BeginFinallyBlock()
                .Invoke(f))
            .EndExceptionBlock();
    }
}