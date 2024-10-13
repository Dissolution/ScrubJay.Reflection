namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public record ThrowExceptionInstruction(Type ExceptionType) : ILGeneratorInstruction;