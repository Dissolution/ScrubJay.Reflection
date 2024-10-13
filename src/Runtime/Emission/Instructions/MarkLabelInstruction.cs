namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public record MarkLabelInstruction(EmitterLabel Label) : ILGeneratorInstruction;