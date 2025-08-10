namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class LocalInstruction : VariableInstruction
{
    public ILLocal? Local { get; internal set; }

    public LocalInstruction(OpCode opCode, int index)
        : base(opCode, index)
    {
        this.Local = null;
    }

    public LocalInstruction(OpCode opCode, ILLocal local)
        : base(opCode, local.Index)
    {
        this.Local = local;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!);
        if (Local.TryGetValue(out var local))
        {
            builder
                // inline none opcodes specify their index in their name (eg ldloc.2)
                .If(OpCode.OperandType != OperandType.InlineNone,
                    tb => tb.Format(Index).Append(": "))
                .Append('`').Render(local).Append('`');
        }
        else
        {
            builder.Append($"[{Index}]");
        }
    }
}