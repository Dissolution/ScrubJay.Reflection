namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class OpCodeValueInstruction<T> : OpCodeInstruction
    where T: unmanaged
{
    public T Value { get; }

    public override int Size
    {
        get
        {
            if (OpCode.OperandType == OperandType.InlineNone)
                return OpCode.Size;
            return OpCode.Size + Notsafe.SizeOf<T>();
        }
    }

    public OpCodeValueInstruction(OpCode opCode, T value)
        : base(opCode)
    {
        this.Value = value;
    }

    public override void RenderTo<B>(B builder)
    {
        base.RenderTo(builder);
        builder.Render(Value);
    }
}