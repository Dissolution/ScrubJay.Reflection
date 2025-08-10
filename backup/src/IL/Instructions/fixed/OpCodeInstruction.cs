namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public class OpCodeInstruction : Instruction,
#if NET7_0_OR_GREATER
    IEqualityOperators<OpCodeInstruction, OpCodeInstruction, bool>,
#endif
    IEquatable<OpCodeInstruction>
{
    public static bool operator ==(OpCodeInstruction? left, OpCodeInstruction? right)
         => Equate.EquatableValues<OpCodeInstruction>(left, right);

    public static bool operator !=(OpCodeInstruction? left, OpCodeInstruction? right)
        => !Equate.EquatableValues<OpCodeInstruction>(left, right);
    
    public override int Size => OpCode.Size;
    
    public OpCode OpCode { get; }

    public bool IsShort =>
        OpCode.OperandType is >= OperandType.ShortInlineBrTarget
            and <= OperandType.ShortInlineVar;

    public bool IsStandard =>
        OpCode.OperandType != OperandType.InlineNone &&
        OpCode.OperandType <= OperandType.InlineVar;

    protected OpCodeInstruction(OpCode opCode, byte _)
    {
        this.OpCode = opCode;
    }
    
    public OpCodeInstruction(OpCode opCode)
    {
        if (opCode.Size == 0)
            throw new ArgumentException("Invalid OpCode", nameof(opCode));
        
        if (opCode.OperandType != OperandType.InlineNone)
            throw new ArgumentException(
                "OpCodeInstruction can only be constructed with non-argument OpCodes",
                nameof(opCode));
        
        /*
         * Add
         * Add_Ovf
         * Add_Ovf_Un
         * And
         * Arglist
         */
        
        this.OpCode = opCode;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Render(OpCode);
    }

    public virtual bool Equals(OpCodeInstruction? other)
    {
        return other is not null && other.OpCode == this.OpCode;
    }
   
    public sealed override bool Equals(Instruction? other)
    {
        return other is OpCodeInstruction opCodeInstruction && Equals(opCodeInstruction);
    }

    public override bool Equals(object? obj)
    {
        return obj is OpCodeInstruction opCodeInstruction && Equals(opCodeInstruction);
    }

    public override int GetHashCode()
    {
        return Hasher.Hash(OpCode);
    }
}