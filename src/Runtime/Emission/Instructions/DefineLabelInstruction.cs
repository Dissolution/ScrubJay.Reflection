namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public record DefineLabelInstruction(EmitterLabel Label) : ILGeneratorInstruction;