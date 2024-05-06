using Dunet;
// ReSharper disable IdentifierTypo

namespace ScrubJay.Reflection.Runtime.Emission;

public sealed record class OpInstruction : Instruction
{
    public OpCode OpCode { get; }

    public object? Value { get; }

    public int Size
    {
        get
        {
            int size = OpCode.Size;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (OpCode.OperandType)
            {
                case OperandType.InlineSwitch:
                {
                    if (!(Value is Instruction[] instructions))
                        throw new InvalidOperationException();
                    size += (1 + instructions.Length) * 4;
                    break;
                }
                case OperandType.InlineI8:
                case OperandType.InlineR:
                    size += 8;
                    break;
                case OperandType.InlineBrTarget:
                case OperandType.InlineField:
                case OperandType.InlineI:
                case OperandType.InlineMethod:
                case OperandType.InlineString:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.ShortInlineR:
                    size += 4;
                    break;
                case OperandType.InlineVar:
                    size += 2;
                    break;
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                    size += 1;
                    break;
            }

            return size;
        }
    }

    public OpInstruction(OpCode opCode, object? value = null)
    {
        OpCode = opCode;
        Value = value;
    }
}

static class Test
{
    static Test()
    {
        Instruction thing = new OpCodeInstruction.Starg_S(156);
    }

}



[Union]
public partial record class OpCodeInstruction : Instruction
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static short ValidateArgIndex(int index,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if (index is <= short.MaxValue and >= 0)
            return (short)index;
        throw new ArgumentOutOfRangeException(indexName, index,
            $"Argument Index must be between 0 and {short.MaxValue}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte ValidateShortArgIndex(
        int index,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if (index is <= byte.MaxValue and >= byte.MinValue)
            return (byte)index;
        throw new ArgumentOutOfRangeException(indexName, index,
            $"Short-form Argument Index must be between {byte.MinValue} and {byte.MaxValue}");
    }

    public OpCode OpCode { get; }

    public virtual int Size => OpCode.Size;

    protected OpCodeInstruction(OpCode opCode)
    {
        this.OpCode = opCode;
    }

    public override string ToString()
    {
        return $"{OpCode.Name}";
    }
}

[Union]
public partial record class OpCodeInstruction<T> : OpCodeInstruction
{
    public T Arg { get; }
    
    protected OpCodeInstruction(OpCode opCode, T arg) : base(opCode)
    {
        this.Arg = arg;
    }
    
    public override string ToString()
    {
        return $"{OpCode.Name} {Arg}";
    }
}

partial record OpCodeInstruction
{

#region TryCatchFinally
    /// <summary>
    /// Transfers control from the filter clause of an exception back to the Common Language Infrastructure (CLI) exception handler.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfilter?view=netcore-3.0"/>
    public partial record Endfilter : OpCodeInstruction
    {
        public Endfilter() : base(OpCodes.Endfilter) { }
    }

    /// <summary>
    /// Transfers control from the fault or finally clause of an exception block back to the Common Language Infrastructure (CLI) exception handler.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfinally"/>
    public partial record Endfinally : OpCodeInstruction
    {
        public Endfinally() : base(OpCodes.Endfinally) { }
    }
#endregion

#region Arguments
    /// <summary>
    /// Returns an unmanaged pointer to the argument list of the current method.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.arglist?view=netcore-3.0"/>
    public partial record Arglist : OpCodeInstruction
    {
        public Arglist() : base(OpCodes.Arglist) { }
    }

#region Load
    /// <summary>
    /// Loads the argument at index 0 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_0"/>
    public partial record Ldarg_0 : OpCodeInstruction
    {
        public Ldarg_0() : base(OpCodes.Ldarg_0) { }
    }

    /// <summary>
    /// Loads the argument at index 1 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_1"/>
    public partial record Ldarg_1 : OpCodeInstruction
    {
        public Ldarg_1() : base(OpCodes.Ldarg_1) { }
    }

    /// <summary>
    /// Loads the argument at index 2 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_2"/>
    public partial record Ldarg_2 : OpCodeInstruction
    {
        public Ldarg_2() : base(OpCodes.Ldarg_2) { }
    }

    /// <summary>
    /// Loads the argument at index 3 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_3"/>
    public partial record Ldarg_3 : OpCodeInstruction
    {
        public Ldarg_3() : base(OpCodes.Ldarg_3) { }
    }

    /// <summary>
    /// Loads the argument with the specified <paramref name="index"/> onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg"/>
    public partial record Ldarg : OpCodeInstruction
    {
        public short Index { get; }

        /// <param name="index"> The index of the argument to load </param>
        /// <exception cref="ArgumentOutOfRangeException"> If <paramref name="index"/> is invalid </exception>
        public Ldarg(int index) : base(OpCodes.Ldarg)
        {
            this.Index = ValidateArgIndex(index);
        }

        public override string ToString()
        {
            return $"{OpCode.Name} {Index}";
        }
    }

    /// <summary>
    /// Loads the argument with the specified short-form <paramref name="index"/> onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_s"/>
    public partial record Ldarg_S : OpCodeInstruction
    {
        public byte Index { get; }

        /// <param name="index">The short-form index of the argument to load.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        public Ldarg_S(int index) : base(OpCodes.Ldarg_S)
        {
            this.Index = ValidateShortArgIndex(index);
        }

        public override string ToString()
        {
            return $"{OpCode.Name} {Index}";
        }
    }

    /// <summary>
    /// Loads the address of the argument with the specified <paramref name="index"/> onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarga"/>
    public partial record Ldarga : OpCodeInstruction
    {
        public short Index { get; }

        /// <param name="index">The index of the argument address to load.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        public Ldarga(int index) : base(OpCodes.Ldarga)
        {
            this.Index = ValidateArgIndex(index);
        }

        public override string ToString()
        {
            return $"{OpCode.Name} {Index}";
        }
    }

    /// <summary>
    /// Loads the address of the argument with the specified short-form <paramref name="index"/> onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarga_s"/>
    public partial record Ldarga_S : OpCodeInstruction
    {
        public byte Index { get; }

        /// <param name="index">The short-form index of the argument address to load.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        public Ldarga_S(int index) : base(OpCodes.Ldarga_S)
        {
            this.Index = ValidateShortArgIndex(index);
        }

        public override string ToString()
        {
            return $"{OpCode.Name} {Index}";
        }
    }
#endregion
#region Store
    /// <summary>
    /// Stores the value on top of the stack in the argument at the given <paramref name="index"/>.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.starg"/>
    public partial record Starg : OpCodeInstruction
    {
        public short Index { get; }

        /// <param name="index">The index of the argument.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="index"/> is invalid
        /// </exception>
        public Starg(int index) : base(OpCodes.Starg)
        {
            this.Index = ValidateArgIndex(index);
        }

        public override string ToString()
        {
            return $"{OpCode.Name} {Index}";
        }
    }

    /// <summary>
    /// Stores the value on top of the stack in the argument at the given short-form <paramref name="index"/>.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.starg_s"/>
    public partial record Starg_S : OpCodeInstruction
    {
        public byte Index { get; }

        /// <param name="index">The short-form index of the argument.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        public Starg_S(int index) : base(OpCodes.Starg_S)
        {
            this.Index = ValidateShortArgIndex(index);
        }

        public override string ToString()
        {
            return $"{OpCode.Name} {Index}";
        }
    }
#endregion
#endregion
}