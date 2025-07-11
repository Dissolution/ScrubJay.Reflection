using ScrubJay.Reflection.IL.Instructions;

namespace ScrubJay.Reflection.IL.Emission;

[PublicAPI]
public interface IEmitter<E> : IFluentBuilder<E>
    where E : IEmitter<E>
{
    IInstructions Instructions { get; }
}