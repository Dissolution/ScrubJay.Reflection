namespace ScrubJay.Reflection.IL.Emission;

[PublicAPI]
public interface ISimpleEmitter<E> : IGenEmitter<E>, IOperationEmitter<E>, IEmitter<E>
    where E : ISimpleEmitter<E>;