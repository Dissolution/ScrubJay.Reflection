namespace ScrubJay.Reflection.Runtime.Emission.Fluent;

public interface IDebugBuilder<TEmitter>
    where TEmitter : IFluentEmitter<TEmitter>
{
    TEmitter Break();
    TEmitter NoOperation();
}