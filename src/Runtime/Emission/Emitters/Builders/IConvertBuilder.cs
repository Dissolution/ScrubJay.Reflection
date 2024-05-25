//namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;
//
//public interface IConvertBuilder<out TEmitter>
//    where TEmitter : IILEmitter<TEmitter>
//{
//    IConvertBuilder<TEmitter> ThrowOnOverflow { get; }
//    IConvertBuilder<TEmitter> Unsigned { get; }
//
//    TEmitter To(Type destType);
//    TEmitter To<T>();
//    TEmitter To(ConvType convType);
//}
//
//public enum ConvType
//{
//    IntPtr,
//    Int8,
//    Int16,
//    Int32,
//    Int64,
//    UIntPtr,
//    UInt8,
//    UInt16,
//    UInt32,
//    UInt64,
//    Single,
//    Double,
//}
//
//internal class ConvertBuilder<TEmitter> : BuilderBase<TEmitter>, IConvertBuilder<TEmitter>
//    where TEmitter : IILEmitter<TEmitter>
//{
//    private bool _throwOnOverflow = false;
//    private bool _unsigned = false;
//
//    public ConvertBuilder(Emitter<TEmitter> emitter) : base(emitter)
//    {
//
//    }
//
//    public IConvertBuilder<TEmitter> ThrowOnOverflow
//    {
//        get
//        {
//            _throwOnOverflow = true;
//            return this;
//        }
//    }
//
//    public IConvertBuilder<TEmitter> Unsigned
//    {
//        get
//        {
//            _unsigned = true;
//            return this;
//        }
//    }
//
//    public TEmitter To(Type destType)
//    {
//        return _emitter.Conv(destType, _unsigned, _throwOnOverflow);
//    }
//
//    public TEmitter To<T>()
//    {
//        return _emitter.Conv<T>(_unsigned, _throwOnOverflow);
//    }
//
//    public TEmitter To(ConvType convType)
//    {
//        return convType switch
//        {
//            ConvType.IntPtr => _emitter.Conv<IntPtr>(),
//            ConvType.Int8 => _emitter.Conv<sbyte>(),
//            ConvType.Int16 => _emitter.Conv<short>(),
//            ConvType.Int32 => _emitter.Conv<int>(),
//            ConvType.Int64 => _emitter.Conv<long>(),
//            ConvType.UIntPtr => _emitter.Conv<UIntPtr>(),
//            ConvType.UInt8 => _emitter.Conv<byte>(),
//            ConvType.UInt16 => _emitter.Conv<ushort>(),
//            ConvType.UInt32 => _emitter.Conv<uint>(),
//            ConvType.UInt64 => _emitter.Conv<ulong>(),
//            ConvType.Single => _emitter.Conv<float>(),
//            ConvType.Double => _emitter.Conv<double>(),
//            _ => throw new ArgumentOutOfRangeException(nameof(convType), convType, null),
//        };
//    }
//}