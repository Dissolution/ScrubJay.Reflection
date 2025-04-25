namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public abstract class OpCodeVarInstruction : OpCodeInstruction
{
    public int Index { get; }

    public override sealed int Size
    {
        get
        {
            if (OpCode.OperandType == OperandType.InlineVar)
                return OpCode.Size + sizeof(ushort);
            if (OpCode.OperandType == OperandType.ShortInlineVar)
                return OpCode.Size + sizeof(byte);
            Debug.Assert(OpCode.OperandType == OperandType.InlineNone);
            return OpCode.Size;
        }
    }

    protected OpCodeVarInstruction(OpCode opCode, int index) : base(opCode)
    {
        switch (opCode.OperandType)
        {
            case OperandType.InlineNone:
            {
                if (index < 0 || index > 3)
                    throw new ArgumentException(null, nameof(index));
                break;
            }
            case OperandType.InlineVar:
            {
                if ((uint)index > (uint)ushort.MaxValue)
                    throw new ArgumentException(null, nameof(index));
                break;
            }
            case OperandType.ShortInlineVar:
            {
                if ((uint)index > (uint)byte.MaxValue)
                    throw new ArgumentException(null, nameof(index));
                break;
            }
            default:
                throw new ArgumentException(null, nameof(opCode));
        }

        this.Index = index;
    }
}

[PublicAPI]
public sealed class OpCodeLocalInstruction : OpCodeVarInstruction
{
    public ILLocal? Local { get; internal set; }

    public OpCodeLocalInstruction(OpCode opCode, int index)
        : base(opCode, index)
    {
        this.Local = null;
    }

    public OpCodeLocalInstruction(OpCode opCode, ILLocal local)
        : base(opCode, local.Index)
    {
        this.Local = local;
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(base.RenderTo!);
        if (Local.TryGetValue(out var local))
        {
            builder
                // inline none opcodes specify their index in their name (eg ldloc.2)
                .If(OpCode.OperandType != OperandType.InlineNone,
                    tb => tb.Append(Index).Append(": "))
                .Append('`').Render(local).Append('`');
        }
        else
        {
            builder.Append('[').Append(Index).Append(']');
        }
    }
}

[PublicAPI]
public sealed class OpCodeParameterInstruction : OpCodeVarInstruction
{
    public ParameterInfo? Parameter { get; internal set; }

    public OpCodeParameterInstruction(OpCode opCode, int index)
        : base(opCode, index)
    {
        if (!opCode.TargetsArgument())
            throw new ArgumentException(null, nameof(opCode));
        this.Parameter = null;
    }
    
    public OpCodeParameterInstruction(OpCode opCode, ParameterInfo parameter)
        : base(opCode, parameter.Position)
    {
        Throw.IfNull(parameter);
        if (!opCode.TargetsArgument())
            throw new ArgumentException(null, nameof(opCode));
        this.Parameter = parameter;
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(base.RenderTo!);
        if (Parameter is not null)
        {
            builder
                // inline none opcodes specify their index in their name (eg ldloc.2)
                .If(OpCode.OperandType != OperandType.InlineNone,
                    tb => tb.Append(Index).Append(": "))
                .Append('`').Render(Parameter).Append('`');
        }
        else
        {
            builder.Append('[').Append(Index).Append(']');
        }
    }
}