//namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;
//
//public interface ICompareBuilder<out TEmitter>
//    where TEmitter : IILEmitter<TEmitter>
//{
//    ICompareBuilder<TEmitter> Unsigned { get; }
//    
//    TEmitter If(CompareOp op);
//}
//
//internal class CompareBuilder<TEmitter> : BuilderBase<TEmitter>, ICompareBuilder<TEmitter>
//    where TEmitter : IILEmitter<TEmitter>
//{
//    private bool _unsigned = false;
//    
//    public CompareBuilder(Emitter<TEmitter> emitter) : base(emitter)
//    {
//    }
//
//    public ICompareBuilder<TEmitter> Unsigned
//    {
//        get
//        {
//            _unsigned = true;
//            return this;
//        }
//    }
//
//    public TEmitter If(CompareOp op)
//    {
//        return op switch
//        {
//            CompareOp.NotEqual => _emitter.Ceq().Not(),
//            CompareOp.Equal => _emitter.Ceq(),
//            CompareOp.GreaterThan => _emitter.Cgt(_unsigned),
//            CompareOp.LessThan => _emitter.Clt(_unsigned),
//            CompareOp.GreaterThanOrEqual => _emitter.Clt(_unsigned).Not(),
//            CompareOp.LessThanOrEqual => _emitter.Cgt(_unsigned).Not(),
//            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null),
//        };
//    }
//}