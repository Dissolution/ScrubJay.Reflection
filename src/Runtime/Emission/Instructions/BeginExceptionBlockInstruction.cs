namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public record BeginExceptionBlockInstruction(EmitterLabel Label) : ILGeneratorInstruction;