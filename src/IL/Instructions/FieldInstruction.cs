namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class FieldInstruction : TokenInstruction
{
    public FieldInfo? Field { get; internal set; }

    public FieldInstruction(OpCode opCode, int token)
        : base(opCode, token)
    {
        if (opCode.OperandType != OperandType.InlineField &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Field = null;
    }

    public FieldInstruction(OpCode opCode, FieldInfo field)
        : base(opCode, field.MetadataToken)
    {
        if (opCode.OperandType != OperandType.InlineField &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Field = field;
    }

    public FieldInstruction(OpCode opCode, int token, FieldInfo? field)
        : base(opCode, token)
    {
        if (opCode.OperandType != OperandType.InlineField &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Field = field;
    }

    internal protected override TextBuilder RenderArgs(TextBuilder builder)
    {
        return builder.IfNotNull(Field, static (tb, field) => tb.Render(field),
            tb => tb.Append($"&{Token:X8}"));
    }
}