namespace ScrubJay.Reflection.IL.Instructions;

public abstract class OpCodeValueInstruction<T> : OpCodeInstruction
    where T: unmanaged
{
    public T Value { get; }

    public override sealed int Size
    {
        get
        {
            if (OpCode.OperandType == OperandType.InlineNone)
                return OpCode.Size;
            return OpCode.Size + Notsafe.SizeOf<T>();
        }
    }

    protected internal OpCodeValueInstruction(OpCode opCode, T value)
        : base(opCode)
    {
        this.Value = value;
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(b => base.RenderTo(b)).Render(Value);
    }
}

public sealed class OpCodeU8Instruction : OpCodeValueInstruction<byte>
{
    public OpCodeU8Instruction(OpCode opCode, byte value) : base(opCode, value)
    {
        
    }
}

public sealed class OpCodeI8Instruction : OpCodeValueInstruction<sbyte>
{
    public OpCodeI8Instruction(OpCode opCode, sbyte value) : base(opCode, value)
    {
        
    }
}

public sealed class OpCodeI32Instruction : OpCodeValueInstruction<int>
{
    public OpCodeI32Instruction(int value) : base(OpCodes.Ldc_I4, value)
    {
        
    }
    
    public OpCodeI32Instruction(OpCode opCode, int value) : base(opCode, value)
    {
        
    }
}

public sealed class OpCodeI64Instruction : OpCodeValueInstruction<long>
{
    public OpCodeI64Instruction(long value) : base(OpCodes.Ldc_I8, value)
    {
        
    }
    
    public OpCodeI64Instruction(OpCode opCode, long value) : base(opCode, value)
    {
        
    }
}

public sealed class OpCodeF32Instruction : OpCodeValueInstruction<float>
{
    public OpCodeF32Instruction(float value) : base(OpCodes.Ldc_R4, value)
    {
        
    }
    
    public OpCodeF32Instruction(OpCode opCode, float value) : base(opCode, value)
    {
        
    }
}

public sealed class OpCodeF64Instruction : OpCodeValueInstruction<double>
{
    public OpCodeF64Instruction(double value) : base(OpCodes.Ldc_R8, value)
    {
        
    }
    
    public OpCodeF64Instruction(OpCode opCode, double value) : base(opCode, value)
    {
        
    }
}