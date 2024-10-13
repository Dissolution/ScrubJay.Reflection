namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public record BeginCatchBlockInstruction(Type ExceptionType) : ILGeneratorInstruction;