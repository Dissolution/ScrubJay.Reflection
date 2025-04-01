using System.Runtime.InteropServices;

namespace ScrubJay.Reflection.OLD.OLD.Runtime.Emission.Instructions;

public record CallUnmanagedInstruction(CallingConvention Convention, Type? ReturnType, Type[]? ParameterTypes) : ILGeneratorInstruction;