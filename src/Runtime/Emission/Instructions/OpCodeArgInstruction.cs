using ScrubJay.Reflection.Runtime.Naming;
// ReSharper disable IdentifierTypo

namespace ScrubJay.Reflection.Runtime.Emission.Instructions;



public record class OpCodeArgInstruction<T> : OpCodeInstruction
{
    public T Arg { get; }

    public override int Size
    {
        get
        {
            int size = OpCode.Size;
            
            switch (OpCode.OperandType)
            {
                case OperandType.InlineSwitch:
                {
                    if (Arg is not Instruction[] instructions)
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
                case OperandType.InlineSig:
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
                case OperandType.InlinePhi:
                case OperandType.InlineNone:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return size;
        }
    }

    public OpCodeArgInstruction(OpCode opCode, T arg) : base(opCode)
    {
        this.Arg = arg;
    }

    public override string ToString() => InterpolateDeeper.Resolve($"{OpCode} {Arg}");
}
/*
public record class OpCodeWithUInt8Instruction : OpCodeInstruction
{
    public byte Value { get; }

    public OpCodeWithUInt8Instruction(OpCode opCode, byte value) : base(opCode)
    {
        this.Value = value;
    }
}

public record class OpCodeWithInt8Instruction : OpCodeInstruction
{
    public sbyte Value { get; }

    public OpCodeWithInt8Instruction(OpCode opCode, sbyte value) : base(opCode)
    {
        this.Value = value;
    }
}

public record class OpCodeWithInt16Instruction : OpCodeInstruction
{
    public short Value { get; }

    public OpCodeWithInt16Instruction(OpCode opCode, short value) : base(opCode)
    {
        this.Value = value;
    }
}


public record class OpCodeWithInt32Instruction : OpCodeInstruction
{
    public int Value { get; }

    public OpCodeWithInt32Instruction(OpCode opCode, int value) : base(opCode)
    {
        this.Value = value;
    }
}

public record class OpCodeWithInt64Instruction : OpCodeInstruction
{
    public long Value { get; }

    public OpCodeWithInt64Instruction(OpCode opCode, long value) : base(opCode)
    {
        this.Value = value;
    }
}

public record class OpCodeWithF32Instruction : OpCodeInstruction
{
    public float Value { get; }

    public OpCodeWithF32Instruction(OpCode opCode, float value) : base(opCode)
    {
        this.Value = value;
    }
}

public record class OpCodeWithF64Instruction : OpCodeInstruction
{
    public double Value { get; }

    public OpCodeWithF64Instruction(OpCode opCode, double value) : base(opCode)
    {
        this.Value = value;
    }
}

public record class OpCodeWithStringInstruction : OpCodeInstruction
{
    public string Value { get; }

    public OpCodeWithStringInstruction(OpCode opCode, string value) : base(opCode)
    {
        this.Value = value;
    }
}

public record class OpCodeWithLabelInstruction : OpCodeInstruction
{
    public EmitterLabel Value { get; }

    public OpCodeWithLabelInstruction(OpCode opCode, EmitterLabel value) : base(opCode)
    {
        this.Value = value;
    }
}

public record class OpCodeWithLabelsInstruction : OpCodeInstruction
{
    public EmitterLabel[] Value { get; }

    public OpCodeWithLabelsInstruction(OpCode opCode, EmitterLabel[] value) : base(opCode)
    {
        this.Value = value;
    }
}

public record class OpCodeWithLocalInstruction : OpCodeInstruction
{
    public EmitterLocal Value { get; }

    public OpCodeWithLocalInstruction(OpCode opCode, EmitterLocal value) : base(opCode)
    {
        this.Value = value;
    }
}

public record class OpCodeWithFieldInstruction : OpCodeInstruction
{
    public FieldInfo Value { get; }

    public OpCodeWithFieldInstruction(OpCode opCode, FieldInfo value) : base(opCode)
    {
        this.Value = value;
    }
}

public record class OpCodeWithConstructorInstruction : OpCodeInstruction
{
    public FieldInfo Value { get; }

    public OpCodeWithFieldInstruction(OpCode opCode, FieldInfo value) : base(opCode)
    {
        this.Value = value;
    }
}
*/