namespace ScrubJay.Reflection.OLD.OLD.Runtime.Emission.Instructions;

public record ThrowExceptionInstruction(Type ExceptionType) : ILGeneratorInstruction;