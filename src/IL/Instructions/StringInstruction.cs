namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class StringInstruction : TokenInstruction
{
    public string? String { get; internal set; }

    public StringInstruction(OpCode opCode, int token) 
        : base(opCode, token)
    {
        if (opCode != OpCodes.Ldstr)
            throw new ArgumentException(null, nameof(opCode));
        String = null;
    }
    
    public StringInstruction(OpCode opCode, int token, string str) 
        : base(opCode, token)
    {
        if (opCode != OpCodes.Ldstr)
            throw new ArgumentException(null, nameof(opCode));
        String = str;
    }

    public StringInstruction(OpCode opCode, string str)
        : base(opCode, -1)
    {
        if (opCode != OpCodes.Ldstr)
            throw new ArgumentException(null, nameof(opCode));
        String = str;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(b => base.RenderTo(b))
            .IfNotNull(String,
                static (tb, str) => tb.Render(str),
                tb => tb.Append('&').Format(Token, "X8"));
    }
}