namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public record CallUnmanagedInstruction(CallingConvention Convention, Type? ReturnType, Type[]? ParameterTypes) : ILGeneratorInstruction;