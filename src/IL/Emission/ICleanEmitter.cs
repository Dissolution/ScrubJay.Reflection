//using ScrubJay.Reflection.IL.LabelOffSetManagement;
//
//namespace ScrubJay.Reflection.IL.Emission;
//
//[PublicAPI]
//public interface ICleanEmitter<E> : IEmitter<E>
//    where E : ICleanEmitter<E>
//{
//    E Math(MathOp op, bool overflowCheck = false, bool unsigned = false);
//    E Bitwise(BitwiseOp op);
//
//    E Call(MethodBase method);
//    E Branch(CompareOp op, ILLabel label, bool unsigned = false);
//    E Branch(bool boolean, ILLabel label);
//
//    E Return();
//    E Box<T>();
//    E Castclass<C>()
//        where C : class;
//    E Unbox<T>();
//    
//    
//    // try/catch/finally
//    E Leave(ILLabel label);
//}