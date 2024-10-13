namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public abstract record ILGeneratorInstruction : Instruction
{
    public override int Size => 0;
}