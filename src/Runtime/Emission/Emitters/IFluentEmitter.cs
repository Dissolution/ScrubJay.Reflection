namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

public interface IFluentEmitter<TEmitter> : IILEmitter<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    TEmitter If(bool predicate, Action<TEmitter>? emitIfTrue, Action<TEmitter>? emitIfFalse = null);
    
    TEmitter IfNotNull<T>(T? value, Action<TEmitter, T> emitIfNotNull);

    bool CanPush<T>(T? value);
    
    Result<TEmitter, Error> TryPush<T>(T? value);
    
    TEmitter Throw(Type exceptionType, params object?[] args);
    TEmitter Throw<TException>(params object?[] args) where TException : Exception;
    
    /// <summary>
    /// Invoke the given <paramref name="emission"/> action on this <typeparamref name="TEmitter"/> instance
    /// </summary>
    /// <param name="emission">The <see cref="Action{T}"/> to perform on this <typeparamref name="TEmitter"/> instance</param>
    /// <returns>
    /// This <typeparamref name="TEmitter"/> instance after <paramref name="emission"/> has been invoked
    /// </returns>
    TEmitter Invoke(Action<TEmitter> emission);
    
    /// <summary>
    /// Invoke the given <paramref name="emission"/> function on this <typeparamref name="TEmitter"/> instance
    /// </summary>
    /// <param name="emission">The <see cref="Func{T,T}"/> to perform on this <typeparamref name="TEmitter"/> instance</param>
    /// <returns>
    /// This <typeparamref name="TEmitter"/> instance after <paramref name="emission"/> has been invoked
    /// </returns>
    TEmitter Invoke(Func<TEmitter, TEmitter> emission);
}