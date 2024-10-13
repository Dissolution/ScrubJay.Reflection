namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public record WriteLineInstruction(object? Value) : ILGeneratorInstruction;