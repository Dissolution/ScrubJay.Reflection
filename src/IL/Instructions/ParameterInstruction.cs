namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class ParameterInstruction : VariableInstruction
{
    public ParameterInfo? Parameter { get; internal set; }

    public ParameterInstruction(OpCode opCode, int index)
        : base(opCode, index)
    {
        if (!opCode.TargetsArgument())
            throw new ArgumentException(null, nameof(opCode));
        this.Parameter = null;
    }
    
    public ParameterInstruction(OpCode opCode, ParameterInfo parameter)
        : base(opCode, parameter.Position)
    {
        Throw.IfNull(parameter);
        if (!opCode.TargetsArgument())
            throw new ArgumentException(null, nameof(opCode));
        this.Parameter = parameter;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!);
        if (Parameter is not null)
        {
            builder
                // inline none opcodes specify their index in their name (eg ldloc.2)
                .If(OpCode.OperandType != OperandType.InlineNone,
                    tb => tb.Format(Index).Append(": "))
                .Append('`').Render(Parameter).Append('`');
        }
        else
        {
            builder.Append('[').Format(Index).Append(']');
        }
    }
}