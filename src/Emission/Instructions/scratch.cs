//namespace ScrubJay.Reflection.Scratch;
//
//// static parts
//
//partial class Instruction
//{
//    public static bool operator ==(Instruction? left, Instruction? right) => Equate.EquatableValues(left, right);
//    public static bool operator !=(Instruction? left, Instruction? right) => !Equate.EquatableValues(left, right);
//    
//    partial class Op
//    {
//        public static bool operator ==(Op? left, Op? right) => Equate.EquatableValues(left, right);
//        public static bool operator !=(Op? left, Op? right) => !Equate.EquatableValues(left, right);
//    }
//}
//
//
//public abstract partial class Instruction :
//#if NET7_0_OR_GREATER
//    IEqualityOperators<Instruction, Instruction, bool>,
//#endif
//    IEquatable<Instruction>
//{
//
//    public bool Equals(Instruction? other) => throw new NotImplementedException();
//}
//
//
//partial class Instruction
//{
//    public abstract partial class Op : Instruction,
//#if NET7_0_OR_GREATER
//        IEqualityOperators<Op, Op, bool>,
//#endif
//        IEquatable<Op>
//    {
//
//        public bool Equals(Op? other) => throw new NotImplementedException();
//    }
//}