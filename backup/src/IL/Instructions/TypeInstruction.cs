namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class TypeInstruction : TokenInstruction
{
    public Type? Type { get; internal set; }

    public TypeInstruction(OpCode opCode, int token)
        : base(opCode, token)
    {
        if (opCode.OperandType != OperandType.InlineType &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Type = null;
    }

    public TypeInstruction(OpCode opCode, Type type)
        : base(opCode, type.MetadataToken)
    {
        if (opCode.OperandType != OperandType.InlineType &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Type = type;
    }
    
    public TypeInstruction(OpCode opCode, int token, Type? type)
        : base(opCode, token)
    {
        if (opCode.OperandType != OperandType.InlineType &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Type = type;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .IfNotNull(Type,
                static (tb, type) => tb.Append('`').Render(type).Append('`'),
                tb => tb.Append('&').Format(Token, "X8"));
    }
}