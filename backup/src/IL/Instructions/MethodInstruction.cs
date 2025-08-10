namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class MethodInstruction : TokenInstruction
{
    public MethodBase? Method { get; internal set; }

    public MethodInstruction(OpCode opCode, int token) 
        : base(opCode, token)
    {
        if (opCode.OperandType != OperandType.InlineMethod &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Method = null;
    }

    public MethodInstruction(OpCode opCode, MethodBase method) 
        : base(opCode, method.MetadataToken)
    {
        if (opCode.OperandType != OperandType.InlineMethod &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Method = method;
    }
    
    public MethodInstruction(OpCode opCode, int token, MethodBase? method) 
        : base(opCode, token)
    {
        if (opCode.OperandType != OperandType.InlineMethod &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Method = method;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .IfNotNull(Method,
                static (tb, method) => tb.Append('`').Render(method).Append('`'),
                tb => tb.Append('&').Format(Token, "X8"));
    }
}