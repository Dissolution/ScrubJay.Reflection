namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Convert a value on the stack to the given non-character primitive type.
    ///
    /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr).
    /// </summary>
    public Emit Convert<TPrimitiveType>()
        where TPrimitiveType : struct
    {
        _innerEmit.Convert<TPrimitiveType>();
        return this;
    }

    /// <summary>
    /// Convert a value on the stack to the given non-character primitive type.
    ///
    /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr).
    /// </summary>
    public Emit Convert(Type primitiveType)
    {
        _innerEmit.Convert(primitiveType);
        return this;
    }

    /// <summary>
    /// Convert a value on the stack to the given non-character, non-float, non-double primitive type.
    /// If the conversion would overflow at runtime, an OverflowException is thrown.
    ///
    /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr).
    /// </summary>
    public Emit ConvertOverflow<TPrimitiveType>()
    {
        _innerEmit.ConvertOverflow<TPrimitiveType>();
        return this;
    }

    /// <summary>
    /// Convert a value on the stack to the given non-character, non-float, non-double primitive type.
    /// If the conversion would overflow at runtime, an OverflowException is thrown.
    ///
    /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr).
    /// </summary>
    public Emit ConvertOverflow(Type primitiveType)
    {
        _innerEmit.ConvertOverflow(primitiveType);
        return this;
    }

    /// <summary>
    /// Convert a value on the stack to the given non-character, non-float, non-double primitive type as if it were unsigned.
    /// If the conversion would overflow at runtime, an OverflowException is thrown.
    ///
    /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr).
    /// </summary>
    public Emit UnsignedConvertOverflow<TPrimitiveType>()
    {
        _innerEmit.UnsignedConvertOverflow<TPrimitiveType>();
        return this;
    }

    /// <summary>
    /// Convert a value on the stack to the given non-character, non-float, non-double primitive type as if it were unsigned.
    /// If the conversion would overflow at runtime, an OverflowException is thrown.
    ///
    /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr).
    /// </summary>
    public Emit UnsignedConvertOverflow(Type primitiveType)
    {
        _innerEmit.UnsignedConvertOverflow(primitiveType);
        return this;
    }

    /// <summary>
    /// Converts a primitive type on the stack to a float, as if it were unsigned.
    ///
    /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr).
    /// </summary>
    public Emit UnsignedConvertToFloat()
    {
        _innerEmit.UnsignedConvertToFloat();
        return this;
    }
}
