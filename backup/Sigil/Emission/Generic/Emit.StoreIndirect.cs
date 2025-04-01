namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pops a value of the given type and a pointer off the stack, and stores the value at the address in the pointer.
    /// </summary>
    public Emit<TDelegateType> StoreIndirect<T>(bool isVolatile = false, int? unaligned = null)
    {
        return StoreIndirect(typeof(T), isVolatile, unaligned);
    }

    /// <summary>
    /// Pops a value of the given type and a pointer off the stack, and stores the value at the address in the pointer.
    /// </summary>
    public Emit<TDelegateType> StoreIndirect(Type type, bool isVolatile = false, int? unaligned = null)
    {
        if (type == null)
        {
            throw new ArgumentNullException("type");
        }

        if (unaligned.HasValue && (unaligned != 1 && unaligned != 2 && unaligned != 4))
        {
            throw new ArgumentException("unaligned must be null, 1, 2, or 4");
        }

        if (isVolatile)
        {
            UpdateState(OpCodes.Volatile, Wrap(StackTransition.None(), "StoreIndirect"));
        }

        if (unaligned.HasValue)
        {
            UpdateState(OpCodes.Unaligned, (byte)unaligned.Value, Wrap(StackTransition.None(), "StoreIndirect"));
        }

        if (type.IsPointer)
        {
            var transition = new[] { new StackTransition(new[] { typeof(NativeIntType), typeof(NativeIntType) }, []) };

            UpdateState(OpCodes.Stind_I, Wrap(transition, "StoreIndirect"));
            return this;
        }

        if (!type.IsValueType)
        {
            var transition =
                new[]
                {
                    new StackTransition(new[] { type, type.MakePointerType() }, []),
                    new StackTransition(new[] { type, type.MakeByRefType() }, []),
                    new StackTransition(new[] { type, typeof(NativeIntType) }, []),
                };

            UpdateState(OpCodes.Stind_Ref, Wrap(transition, "StoreIndirect"));
            return this;
        }

        if (type == typeof(sbyte) || type == typeof(byte))
        {
            var transition =
                new[]
                {
                    new StackTransition(new[] { typeof(int), typeof(byte*) }, []),
                    new StackTransition(new[] { typeof(int), typeof(byte).MakeByRefType() }, []),
                    new StackTransition(new[] { typeof(int), typeof(sbyte*) }, []),
                    new StackTransition(new[] { typeof(int), typeof(sbyte).MakeByRefType() }, []),
                    new StackTransition(new[] { typeof(int), typeof(NativeIntType) }, []),
                };

            UpdateState(OpCodes.Stind_I1, Wrap(transition, "StoreIndirect"));
            return this;
        }

        if (type == typeof(short) || type == typeof(ushort))
        {
            var transition =
                new[]
                {
                    new StackTransition(new[] { typeof(int), typeof(short*) }, []),
                    new StackTransition(new[] { typeof(int), typeof(short).MakeByRefType() }, []),
                    new StackTransition(new[] { typeof(int), typeof(ushort*) }, []),
                    new StackTransition(new[] { typeof(int), typeof(ushort).MakeByRefType() }, []),
                    new StackTransition(new[] { typeof(int), typeof(NativeIntType) }, []),
                };

            UpdateState(OpCodes.Stind_I2, Wrap(transition, "StoreIndirect"));
            return this;
        }

        if (type == typeof(int) || type == typeof(uint))
        {
            var transition =
                new[]
                {
                    new StackTransition(new[] { typeof(int), typeof(int*) }, []),
                    new StackTransition(new[] { typeof(int), typeof(int).MakeByRefType() }, []),
                    new StackTransition(new[] { typeof(int), typeof(uint*) }, []),
                    new StackTransition(new[] { typeof(int), typeof(uint).MakeByRefType() }, []),
                    new StackTransition(new[] { typeof(int), typeof(NativeIntType) }, []),
                };

            UpdateState(OpCodes.Stind_I4, Wrap(transition, "StoreIndirect"));
            return this;
        }

        if (type == typeof(long) || type == typeof(ulong))
        {
            var transition =
                new[]
                {
                    new StackTransition(new[] { typeof(long), typeof(long*) }, []),
                    new StackTransition(new[] { typeof(long), typeof(long).MakeByRefType() }, []),
                    new StackTransition(new[] { typeof(long), typeof(ulong*) }, []),
                    new StackTransition(new[] { typeof(long), typeof(ulong).MakeByRefType() }, []),
                    new StackTransition(new[] { typeof(long), typeof(NativeIntType) }, []),
                };

            UpdateState(OpCodes.Stind_I8, Wrap(transition, "StoreIndirect"));
            return this;
        }

        if (type == typeof(float))
        {
            var transition =
                new[]
                {
                    new StackTransition(new[] { typeof(float), typeof(float*) }, []),
                    new StackTransition(new[] { typeof(float), typeof(float).MakeByRefType() }, []),
                    new StackTransition(new[] { typeof(float), typeof(NativeIntType) }, []),
                };

            UpdateState(OpCodes.Stind_R4, Wrap(transition, "StoreIndirect"));
            return this;
        }

        if (type == typeof(double))
        {
            var transition =
                new[]
                {
                    new StackTransition(new[] { typeof(double), typeof(double*) }, []),
                    new StackTransition(new[] { typeof(double), typeof(double).MakeByRefType() }, []),
                    new StackTransition(new[] { typeof(double), typeof(NativeIntType) }, []),
                };

            UpdateState(OpCodes.Stind_R8, Wrap(transition, "StoreIndirect"));
            return this;
        }

        throw new InvalidOperationException("StoreIndirect cannot be used with " + type + ", StoreObject may be more appropriate");
    }
}
