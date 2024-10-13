namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public record CallManagedInstruction(CallingConventions Conventions, Type? ReturnType, Type[]? ParameterTypes, Type[]? OptionalParameterTypes = null) : ILGeneratorInstruction;