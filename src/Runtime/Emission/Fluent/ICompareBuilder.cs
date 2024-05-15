namespace ScrubJay.Reflection.Runtime.Emission.Fluent;

public interface ICompareBuilder<TEmitter>
    where TEmitter : IFluentEmitter<TEmitter>
{
    ICompareBuilder<TEmitter> Unsigned { get; }
    
    TEmitter If(CompareOp op);
}