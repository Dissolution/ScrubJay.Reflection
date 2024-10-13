namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public record CallVarargsInstruction(MethodInfo Method, Type[]? OptionalParameterTypes = null) : ILGeneratorInstruction;