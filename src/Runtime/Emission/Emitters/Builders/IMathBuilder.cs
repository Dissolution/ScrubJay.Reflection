//namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;
//
//public interface IMathBuilder<out TEmitter>
//    where TEmitter : IILEmitter<TEmitter>
//{
//    IMathBuilder<TEmitter> ThrowOnOverflow { get; }
//    IMathBuilder<TEmitter> Unsigned { get; }
//
//    TEmitter Add();
//    TEmitter Divide();
//    TEmitter Multiply();
//    TEmitter DivisionRemainder();
//    TEmitter Subtract();
//}
//
//internal class MathBuilder<TEmitter> : BuilderBase<TEmitter>, IMathBuilder<TEmitter>
//    where TEmitter : IILEmitter<TEmitter>
//{
//    private bool _throwOnOverflow = false;
//    private bool _unsigned = false;
//    
//    public MathBuilder(Emitter<TEmitter> emitter) : base(emitter)
//    {
//    }
//    
//    public IMathBuilder<TEmitter> ThrowOnOverflow
//    {
//        get
//        {
//            _throwOnOverflow = true;
//            return this;
//        }
//    }
//
//    public IMathBuilder<TEmitter> Unsigned
//    {
//        get
//        {
//            _unsigned = true;
//            return this;
//        }
//    }
//
//
//    public TEmitter Add() => _emitter.Add(_unsigned, _throwOnOverflow);
//    public TEmitter Divide() => _emitter.Div(_unsigned);
//    public TEmitter Multiply() => _emitter.Mul(_unsigned, _throwOnOverflow);
//    public TEmitter DivisionRemainder() => _emitter.Rem(_unsigned);
//    public TEmitter Subtract() =>_emitter.Sub(_unsigned, _throwOnOverflow);
//}