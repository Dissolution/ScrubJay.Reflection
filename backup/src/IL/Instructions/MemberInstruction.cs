namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class MemberInstruction : TokenInstruction
{
    public MemberInfo? Member { get; internal set; }

    public MemberInstruction(OpCode opCode, int token)
        : base(opCode, token)
    {
        if (opCode != OpCodes.Ldtoken)
            throw new ArgumentException(nameof(opCode));
        this.Member = null;
    }

    public MemberInstruction(OpCode opCode, MemberInfo member)
        : base(opCode, member.MetadataToken)
    {
        if (opCode != OpCodes.Ldtoken)
            throw new ArgumentException(nameof(opCode));
        this.Member = member;
    }
    
    public MemberInstruction(OpCode opCode, int token, MemberInfo? member)
        : base(opCode, token)
    {
        if (opCode != OpCodes.Ldtoken)
            throw new ArgumentException(nameof(opCode));
        this.Member = member;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .IfNotNull(Member,
                static (tb, member) => tb.Append('`').Render(member).Append('`'),
                tb => tb.Append('&').Format(Token, "X8"));
    }
}