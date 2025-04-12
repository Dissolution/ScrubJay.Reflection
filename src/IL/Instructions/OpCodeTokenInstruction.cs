namespace ScrubJay.Reflection.IL.Instructions;

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

public sealed class OpCodeFieldInstruction : OpCodeTokenInstruction
{
    public FieldInfo? Field { get; set; }
    
    public OpCodeFieldInstruction(OpCode opCode, int token) : base(opCode, token)
    {
        Debug.Assert(opCode.OperandType == OperandType.InlineField);
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(b => base.RenderTo(b))
            .If(Validate.IsNotNull(Field),
                static (tb, field) => tb.Append('`').Render(field).Append('`'),
                (tb, _) => tb.Append('&').Append(Token, "X8"));
    }
}

public sealed class OpCodeMethodInstruction : OpCodeTokenInstruction
{
    public MethodBase? Method { get; set; }
    
    public OpCodeMethodInstruction(OpCode opCode, int token) : base(opCode, token)
    {
        Debug.Assert(opCode.OperandType == OperandType.InlineMethod);
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(b => base.RenderTo(b))
            .If(Validate.IsNotNull(Method),
                static (tb, method) => tb.Append('`').Render(method).Append('`'),
                (tb, _) => tb.Append('&').Append(Token, "X8"));
    }
}

public sealed class OpCodeSignatureInstruction : OpCodeTokenInstruction
{
    public byte[]? Signature { get; set; }
    
    public OpCodeSignatureInstruction(int token) : base(OpCodes.Calli, token)
    {

    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(b => base.RenderTo(b))
            .If(Validate.IsNotNull(Signature),
                static (tb, sig) => tb.Append('[').DelimitAppend(',', sig, "X2").Append(']'),
                (tb, _) => tb.Append('&').Append(Token, "X8"));
    }
}

public sealed class OpCodeStringInstruction : OpCodeTokenInstruction
{
    public string? String { get; set; }
    
    public OpCodeStringInstruction(int token) : base(OpCodes.Ldstr, token)
    {
        
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(b => base.RenderTo(b))
            .If(Validate.IsNotNull(String),
                static (tb, str) => tb.Render(str),
                (tb, _) => tb.Append('&').Append(Token, "X8"));
    }
}

public sealed class OpCodeMemberInstruction : OpCodeTokenInstruction
{
    public MemberInfo? Member { get; set; }
    
    public OpCodeMemberInstruction(int token) : base(OpCodes.Ldtoken, token)
    {

    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(b => base.RenderTo(b))
            .If(Validate.IsNotNull(Member),
                static (tb, member) => tb.Append('`').Render(member).Append('`'),
                (tb, _) => tb.Append('&').Append(Token, "X8"));
    }
}

public sealed class OpCodeTypeInstruction : OpCodeTokenInstruction
{
    public Type? Type { get; set; }
    
    public OpCodeTypeInstruction(OpCode opCode, int token) : base(opCode, token)
    {
        Debug.Assert(opCode.OperandType == OperandType.InlineType);
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(b => base.RenderTo(b))
            .If(Validate.IsNotNull(Type),
                static (tb, type) => tb.Append('`').Render(type).Append('`'),
                (tb, _) => tb.Append('&').Append(Token, "X8"));
    }
}