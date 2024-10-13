namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public record DeclareLocalInstruction(EmitterLocal Local) : ILGeneratorInstruction;