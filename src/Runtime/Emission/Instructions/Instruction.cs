namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public abstract record class Instruction
{
    public abstract int Size { get; }
}