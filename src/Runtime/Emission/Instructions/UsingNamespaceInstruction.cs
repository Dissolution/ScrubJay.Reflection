namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public record UsingNamespaceInstruction(string Namespace) : ILGeneratorInstruction;