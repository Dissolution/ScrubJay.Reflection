namespace ScrubJay.Reflection.IL.Instructions;

public abstract class OpCodeVariableInstruction : OpCodeInstruction
{
    public int Index { get; }

    public override sealed int Size
    {
        get
        {
            if (OpCode.OperandType == OperandType.InlineVar)
                return OpCode.Size + sizeof(ushort);
            if (OpCode.OperandType == OperandType.ShortInlineVar)
                return OpCode.Size + sizeof(byte);
            Debug.Assert(OpCode.OperandType == OperandType.InlineNone);
            return OpCode.Size;
        }
    }
    
    protected OpCodeVariableInstruction(OpCode opCode, int index) : base(opCode)
    {
        if (opCode.OperandType == OperandType.InlineVar)
        {
            Debug.Assert(index <= ushort.MaxValue);
        }
        else if (opCode.OperandType == OperandType.ShortInlineVar)
        {
            Debug.Assert(index <= byte.MaxValue);
        }
        else if (opCode.OperandType == OperandType.InlineNone)
        {
            Debug.Assert(index >= 0 && index <= ushort.MaxValue);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(opCode));
        }
        this.Index = index;
    }
}

public sealed class OpCodeLocalInstruction : OpCodeVariableInstruction
{
    public CILLocal? Local { get; set; }
    
    public OpCodeLocalInstruction(OpCode opCode, int index) : base(opCode, index)
    {
        
    }

    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(b => base.RenderTo(b))
            .Append('`')
            .If(Local,
                static (tb, local) => local.RenderTo(tb),
                tb => tb.Append('[')
                    .Render(IsShort ? (byte)Index : (ushort)Index)
                    .Append(']'))
            .Append('`');
    }
}

public class OpCodeParameterInstruction : OpCodeVariableInstruction
{
    public ParameterInfo? Parameter { get; set; }
    
    public OpCodeParameterInstruction(OpCode opCode, int index) : base(opCode, index)
    {
        
    }
    
    public override void RenderTo<B>(B builder)
    {
        builder.Invoke(b => base.RenderTo(b))
            .Append('`')
            .IfNotNull(Parameter,
                static (tb, parameter) => tb.Render(parameter),
                tb => tb.Append('[')
                    .Render(IsShort ? (byte)Index : (ushort)Index)
                    .Append(']'))
            .Append('`');
    }
}