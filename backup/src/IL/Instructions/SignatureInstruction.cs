namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class SignatureInstruction : TokenInstruction
{
    public byte[]? Signature { get; internal set; }

    public SignatureInstruction(OpCode opCode, int token)
        : base(opCode, token)
    {
        if (opCode != OpCodes.Calli)
            throw new ArgumentException(null, nameof(opCode));
        this.Signature = null;
    }
    
    public SignatureInstruction(OpCode opCode, int token, byte[] signature)
        : base(opCode, token)
    {
        if (opCode != OpCodes.Calli)
            throw new ArgumentException(null, nameof(opCode));
        this.Signature = signature;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .IfNotNull(Signature,
                static (tb, sig) => tb
                    .Append('[')
                    .EnumerateAndDelimit(sig, static (tb,s) => tb.Format(s, "X2"), ',')
                    .Append(']'),
                tb => tb.Append('&').Format(Token, "X8"));
    }
}