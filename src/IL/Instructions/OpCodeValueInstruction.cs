namespace ScrubJay.Reflection.IL.Instructions;

public abstract class OpCodeValueInstruction<T> : OpCodeInstruction
    where T: unmanaged
{
    public required T Value { get; init; }

    public override sealed int Size => OpCode.Size + Notsafe.SizeOf<T>();

    [SetsRequiredMembers]
    protected internal OpCodeValueInstruction(OpCode opCode, T value)
        : base(opCode)
    {
        this.Value = value;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo).Render(Value);
    }
}

public sealed class OpCodeU8Instruction : OpCodeValueInstruction<byte>
{
    [SetsRequiredMembers]
    public OpCodeU8Instruction(OpCode opCode, byte value) : base(opCode, value)
    {
        
    }
}

public sealed class OpCodeI8Instruction : OpCodeValueInstruction<sbyte>
{
    [SetsRequiredMembers]
    public OpCodeI8Instruction(OpCode opCode, sbyte value) : base(opCode, value)
    {
        
    }
}

public sealed class OpCodeI32Instruction : OpCodeValueInstruction<int>
{
    [SetsRequiredMembers]
    public OpCodeI32Instruction(int value) : base(OpCodes.Ldc_I4, value)
    {
        
    }
}

public sealed class OpCodeI64Instruction : OpCodeValueInstruction<long>
{
    [SetsRequiredMembers]
    public OpCodeI64Instruction(long value) : base(OpCodes.Ldc_I8, value)
    {
        
    }
}

public sealed class OpCodeF32Instruction : OpCodeValueInstruction<float>
{
    [SetsRequiredMembers]
    public OpCodeF32Instruction(float value) : base(OpCodes.Ldc_R4, value)
    {
        
    }
}

public sealed class OpCodeF64Instruction : OpCodeValueInstruction<double>
{
    [SetsRequiredMembers]
    public OpCodeF64Instruction(double value) : base(OpCodes.Ldc_R8, value)
    {
        
    }
}