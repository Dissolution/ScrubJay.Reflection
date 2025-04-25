namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public abstract class OpCodeTokenInstruction : OpCodeInstruction
{
    public int Token { get; }

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
    public FieldInfo? Field { get; internal set; }

    public OpCodeFieldInstruction(OpCode opCode, int token)
        : base(opCode, token)
    {
        if (opCode.OperandType != OperandType.InlineField &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Field = null;
    }

    public OpCodeFieldInstruction(OpCode opCode, FieldInfo field)
        : base(opCode, field.MetadataToken)
    {
        if (opCode.OperandType != OperandType.InlineField &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Field = field;
    }

    public OpCodeFieldInstruction(OpCode opCode, int token, FieldInfo? field)
        : base(opCode, token)
    {
        if (opCode.OperandType != OperandType.InlineField &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Field = field;
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
    public MethodBase? Method { get; internal set; }

    public OpCodeMethodInstruction(OpCode opCode, int token) 
        : base(opCode, token)
    {
        if (opCode.OperandType != OperandType.InlineMethod &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Method = null;
    }

    public OpCodeMethodInstruction(OpCode opCode, MethodBase method) 
        : base(opCode, method.MetadataToken)
    {
        if (opCode.OperandType != OperandType.InlineMethod &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Method = method;
    }
    
    public OpCodeMethodInstruction(OpCode opCode, int token, MethodBase? method) 
        : base(opCode, token)
    {
        if (opCode.OperandType != OperandType.InlineMethod &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Method = method;
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
    public MemberInfo? Member { get; internal set; }

    public OpCodeMemberInstruction(OpCode opCode, int token)
        : base(opCode, token)
    {
        if (opCode != OpCodes.Ldtoken)
            throw new ArgumentException(nameof(opCode));
        this.Member = null;
    }

    public OpCodeMemberInstruction(OpCode opCode, MemberInfo member)
        : base(opCode, member.MetadataToken)
    {
        if (opCode != OpCodes.Ldtoken)
            throw new ArgumentException(nameof(opCode));
        this.Member = member;
    }
    
    public OpCodeMemberInstruction(OpCode opCode, int token, MemberInfo? member)
        : base(opCode, token)
    {
        if (opCode != OpCodes.Ldtoken)
            throw new ArgumentException(nameof(opCode));
        this.Member = member;
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
    public Type? Type { get; internal set; }

    public OpCodeTypeInstruction(OpCode opCode, int token)
        : base(opCode, token)
    {
        if (opCode.OperandType != OperandType.InlineType &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Type = null;
    }

    public OpCodeTypeInstruction(OpCode opCode, Type type)
        : base(opCode, type.MetadataToken)
    {
        if (opCode.OperandType != OperandType.InlineType &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Type = type;
    }
    
    public OpCodeTypeInstruction(OpCode opCode, int token, Type? type)
        : base(opCode, token)
    {
        if (opCode.OperandType != OperandType.InlineType &&
            opCode.OperandType != OperandType.InlineTok)
            throw new ArgumentException(null, nameof(opCode));
        this.Type = type;
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
    public byte[]? Signature { get; internal set; }

    public OpCodeSignatureInstruction(OpCode opCode, int token)
        : base(opCode, token)
    {
        if (opCode != OpCodes.Calli)
            throw new ArgumentException(null, nameof(opCode));
        this.Signature = null;
    }
    
    public OpCodeSignatureInstruction(OpCode opCode, int token, byte[] signature)
        : base(opCode, token)
    {
        if (opCode != OpCodes.Calli)
            throw new ArgumentException(null, nameof(opCode));
        this.Signature = signature;
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
    public string? String { get; internal set; }

    public OpCodeStringInstruction(OpCode opCode, int token) 
        : base(opCode, token)
    {
        if (opCode != OpCodes.Ldstr)
            throw new ArgumentException(null, nameof(opCode));
        String = null;
    }
    
    public OpCodeStringInstruction(OpCode opCode, int token, string str) 
        : base(opCode, token)
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