namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public abstract class OpCodeVariableInstruction : OpCodeInstruction
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
    
    protected OpCodeVariableInstruction(OpCode opCode, int index) : base(opCode)
    {
        if (opCode.OperandType == OperandType.InlineVar)
        {
            Debug.Assert(index <= ushort.MaxValue);
        }
        else if (opCode.OperandType == OperandType.ShortInlineVar)
        {
            Debug.Assert(index <= byte.MaxValue);
        }
        else if (opCode.OperandType == OperandType.InlineNone)
        {
            Debug.Assert(index >= 0 && index <= ushort.MaxValue);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(opCode));
        }
        this.Index = index;
    }
}

[PublicAPI]
public sealed class OpCodeLocalInstruction : OpCodeVariableInstruction
{
    public ILLocal Local { get; }
    
    public OpCodeLocalInstruction(OpCode opCode, ILLocal local) : base(opCode, local.Index)
    {
        
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(base.RenderTo!)
            // inline none opcodes specify their index in their name (eg ldloc.2)
            .If(OpCode.OperandType != OperandType.InlineNone,
                tb => tb.Append(Index).Append(": "))
            .Append('`').Render(Local).Append('`');
    }
}

[PublicAPI]
public sealed class OpCodeParameterInstruction : OpCodeVariableInstruction
{
    public ParameterInfo Parameter { get; }
    
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
        builder.Invoke(base.RenderTo!)
            // inline none opcodes specify their index in their name (eg ldloc.2)
            .If(OpCode.OperandType != OperandType.InlineNone,
                tb => tb.Append(Index).Append(": "))
            .Append('`').Render(Parameter).Append('`');
    }
}