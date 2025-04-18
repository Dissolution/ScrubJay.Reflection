namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public abstract class OpCodeTokenInstruction : OpCodeInstruction
{
    public int Token { get;  }

    public override sealed int Size => OpCode.Size + sizeof(int);
    
    protected internal OpCodeTokenInstruction(OpCode opCode, int token)
        : base(opCode)
    {
        this.Token = token;
    }
}

[PublicAPI]
public sealed class OpCodeFieldInstruction : OpCodeTokenInstruction
{
    public FieldInfo Field { get; }
    
    public OpCodeFieldInstruction(OpCode opCode, FieldInfo field) : base(opCode, field.MetadataToken)
    {
        if (opCode.OperandType != OperandType.InlineField &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Field = field.ThrowIfNull();
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(base.RenderTo!)
            .IfNotNull(Field,
                static (tb, field) => tb.Append('`').Render(field).Append('`'),
                tb => tb.Append('&').Append(Token, "X8"));
    }
}

[PublicAPI]
public sealed class OpCodeMethodInstruction : OpCodeTokenInstruction
{
    public MethodBase Method { get; }
    
    public OpCodeMethodInstruction(OpCode opCode, MethodBase method) : base(opCode, method.MetadataToken)
    {
        if (opCode.OperandType != OperandType.InlineMethod &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Method = method.ThrowIfNull();
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(base.RenderTo!)
            .IfNotNull(Method,
                static (tb, method) => tb.Append('`').Render(method).Append('`'),
                tb => tb.Append('&').Append(Token, "X8"));
    }
}

[PublicAPI]
public sealed class OpCodeMemberInstruction : OpCodeTokenInstruction
{
    public MemberInfo Member { get; }
    
    public OpCodeMemberInstruction(OpCode opCode, MemberInfo member) 
        : base(opCode, member.MetadataToken)
    {
        if (opCode != OpCodes.Ldtoken)
            throw new ArgumentException(nameof(opCode));
        this.Member = member.ThrowIfNull();
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(base.RenderTo!)
            .IfNotNull(Member,
                static (tb, member) => tb.Append('`').Render(member).Append('`'),
                tb => tb.Append('&').Append(Token, "X8"));
    }
}

[PublicAPI]
public sealed class OpCodeTypeInstruction : OpCodeTokenInstruction
{
    public Type Type { get; }
    
    public OpCodeTypeInstruction(OpCode opCode, Type type) : base(opCode, type.MetadataToken)
    {
        if (opCode.OperandType != OperandType.InlineType &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Type = type.ThrowIfNull();
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(base.RenderTo!)
            .IfNotNull(Type,
                static (tb, type) => tb.Append('`').Render(type).Append('`'),
                tb => tb.Append('&').Append(Token, "X8"));
    }
}

[PublicAPI]
public sealed class OpCodeSignatureInstruction : OpCodeTokenInstruction
{
    public byte[]? Signature { get; set; }
    
    public OpCodeSignatureInstruction(OpCode opCode, int token) : base(opCode, token)
    {
        Debug.Assert(opCode == OpCodes.Calli);
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(base.RenderTo!)
            .IfNotNull(Signature,
                static (tb, sig) => tb.Append('[').DelimitAppend(',', sig, "X2").Append(']'),
                tb => tb.Append('&').Append(Token, "X8"));
    }
}

[PublicAPI]
public sealed class OpCodeStringInstruction : OpCodeTokenInstruction
{
    public string String { get; }
    
    public OpCodeStringInstruction(OpCode opCode, string str) : base(opCode, str.GetMetadataToken())
    {
        if (opCode != OpCodes.Ldstr)
            throw new ArgumentException(null, nameof(opCode));
        String = str;
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(b => base.RenderTo(b))
            .IfNotNull(String,
                static (tb, str) => tb.Render(str),
                tb => tb.Append('&').Append(Token, "X8"));
    }
}