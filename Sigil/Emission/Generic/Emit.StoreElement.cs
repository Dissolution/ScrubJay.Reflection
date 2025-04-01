namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pops a value, an index, and a reference to an array off the stack.  Places the given value into the given array at the given index.
    /// </summary>
    public Emit<TDelegateType> StoreElement<TElementType>()
    {
        return StoreElement(typeof(TElementType));
    }

    /// <summary>
    /// Pops a value, an index, and a reference to an array off the stack.  Places the given value into the given array at the given index.
    /// </summary>
    public Emit<TDelegateType> StoreElement(Type elementType)
    {
        if (elementType == null)
        {
            throw new ArgumentNullException("elementType");
        }

        var arrayType = elementType.MakeArrayType();

        IEnumerable<StackTransition> transitions = null;

        OpCode? instr = null;

        if (elementType.IsPointer)
        {
            transitions =
                new[] {
                    new StackTransition(new [] { elementType, typeof(NativeIntType), arrayType }, []),
                    new StackTransition(new [] { elementType, typeof(int), arrayType }, []),
                };

            instr = OpCodes.Stelem_I;
        }

        if (!elementType.IsValueType && !instr.HasValue)
        {
            transitions =
                new[] {
                    new StackTransition(new [] { elementType, typeof(NativeIntType), arrayType }, []),
                    new StackTransition(new [] { elementType, typeof(int), arrayType }, []),
                };

            instr = OpCodes.Stelem_Ref;
        }

        if ((elementType == typeof(sbyte) || elementType == typeof(byte)) && !instr.HasValue)
        {
            transitions =
                new[] {
                    new StackTransition(new [] { typeof(int), typeof(NativeIntType), arrayType }, []),
                    new StackTransition(new [] { typeof(int), typeof(int), arrayType }, []),
                };

            instr = OpCodes.Stelem_I1;
        }

        if ((elementType == typeof(short) || elementType == typeof(ushort)) && !instr.HasValue)
        {
            transitions =
                new[] {
                    new StackTransition(new [] { typeof(int), typeof(NativeIntType), arrayType }, []),
                    new StackTransition(new [] { typeof(int), typeof(int), arrayType }, []),
                };

            instr = OpCodes.Stelem_I2;
        }

        if ((elementType == typeof(int) || elementType == typeof(uint)) && !instr.HasValue)
        {
            transitions =
                new[] {
                    new StackTransition(new [] { typeof(int), typeof(NativeIntType), arrayType }, []),
                    new StackTransition(new [] { typeof(int), typeof(int), arrayType }, []),
                };

            instr = OpCodes.Stelem_I4;
        }

        if ((elementType == typeof(long) || elementType == typeof(ulong)) && !instr.HasValue)
        {
            transitions =
                new[] {
                    new StackTransition(new [] { typeof(long), typeof(NativeIntType), arrayType }, []),
                    new StackTransition(new [] { typeof(long), typeof(int), arrayType }, []),
                };

            instr = OpCodes.Stelem_I8;
        }

        if (elementType == typeof(float) && !instr.HasValue)
        {
            transitions =
                new[] {
                    new StackTransition(new [] { typeof(float), typeof(NativeIntType), arrayType }, []),
                    new StackTransition(new [] { typeof(float), typeof(int), arrayType }, []),
                };

            instr = OpCodes.Stelem_R4;
        }

        if (elementType == typeof(double) && !instr.HasValue)
        {
            transitions =
                new[] {
                    new StackTransition(new [] { typeof(double), typeof(NativeIntType), arrayType }, []),
                    new StackTransition(new [] { typeof(double), typeof(int), arrayType }, []),
                };

            instr = OpCodes.Stelem_R8;
        }

        if (!instr.HasValue)
        {
            transitions =
                new[] {
                    new StackTransition(new [] { elementType, typeof(NativeIntType), arrayType }, []),
                    new StackTransition(new [] { elementType, typeof(int), arrayType }, []),
                };

            UpdateState(OpCodes.Stelem, elementType, Wrap(transitions, "StoreElement"));
            return this;
        }

        UpdateState(instr.Value, Wrap(transitions, "StoreElement"));

        return this;
    }
}
