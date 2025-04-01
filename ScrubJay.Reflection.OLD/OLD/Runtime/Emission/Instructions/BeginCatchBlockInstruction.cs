namespace ScrubJay.Reflection.OLD.OLD.Runtime.Emission.Instructions;

public record BeginCatchBlockInstruction(Type ExceptionType) : ILGeneratorInstruction;