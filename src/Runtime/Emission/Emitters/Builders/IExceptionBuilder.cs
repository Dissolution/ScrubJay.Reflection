namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

public interface IExceptionBuilder<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    TEmitter Throw();
    TEmitter Throw(Type exceptionType, params object?[] args);
    TEmitter Throw<TException>(params object?[] args) where TException : Exception;
}