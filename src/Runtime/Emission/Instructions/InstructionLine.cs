namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public sealed record class InstructionLine(int? Offset, Instruction Instruction)
{
    public override string ToString()
    {
        return $"IL_{Offset:X4}: {Instruction}";
    }
}