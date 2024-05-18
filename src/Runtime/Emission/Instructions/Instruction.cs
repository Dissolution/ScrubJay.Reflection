namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

//https://github.com/0xd4d/dnlib/blob/master/src/DotNet/Emit/Instruction.cs

public abstract record class Instruction
{
    public abstract int Size { get; }
}