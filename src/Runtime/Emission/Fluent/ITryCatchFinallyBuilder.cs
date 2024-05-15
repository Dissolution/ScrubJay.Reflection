namespace ScrubJay.Reflection.Runtime.Emission.Fluent;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEmitter">The <see cref="IILEmitter"/> that this <c>try</c>/<c>catch</c> builder emits to</typeparam>
public interface ITryCatchFinallyBuilder<TEmitter>
    where TEmitter : IFluentEmitter<TEmitter>
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
    ITryCatchFinallyBuilder<TEmitter> Catch(Type exceptionType, Action<ICatchBlockBuilder<TEmitter>> emitCatchBlock);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emitCatchBlock"></param>
    /// <typeparam name="TException"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="emitCatchBlock"/> is <c>null</c></exception>
    ITryCatchFinallyBuilder<TEmitter> Catch<TException>(Action<ICatchBlockBuilder<TEmitter>> emitCatchBlock)
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

//internal sealed class TryCatchFinallyBuilder<TEmitter> : ITryCatchFinallyBuilder<TEmitter>
//    where TEmitter : IILEmitter<TEmitter>
//{
//    private readonly TEmitter _emitter;
//    private EmitterLabel? _endTryLabel;
//
//    public EmitterLabel? EndTryLabel => _endTryLabel;
//    
//    public TryCatchFinallyBuilder(TEmitter emitter)
//    {
//        _emitter = emitter;
//    }
//
//    public ITryCatchFinallyBuilder<TEmitter> Try(Action<TEmitter> tryBlock)
//    {
//        _emitter.BeginExceptionBlock(out _endTryLabel);
//        tryBlock(_emitter);
//        return this;
//    }
//
//    public ITryCatchFinallyBuilder<TEmitter> Catch(Type exceptionType, Action<TEmitter> catchBlock)
//    {
//        _emitter.BeginCatchBlock(exceptionType);
//        catchBlock(_emitter);
//        return this;
//    }
//
//    public TEmitter Finally() => _emitter;
//    
//    public TEmitter Finally(Action<TEmitter>? finallyBlock)
//    {
//        if (finallyBlock is not null)
//        {
//            _emitter.BeginFinallyBlock();
//            finallyBlock(_emitter);
//            _emitter.EndExceptionBlock();
//        }
//        return _emitter;
//    }
//}