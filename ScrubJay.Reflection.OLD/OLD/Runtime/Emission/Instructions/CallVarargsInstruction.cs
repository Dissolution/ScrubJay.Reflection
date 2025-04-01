using System.Reflection;

namespace ScrubJay.Reflection.OLD.OLD.Runtime.Emission.Instructions;

public record CallVarargsInstruction(MethodInfo Method, Type[]? OptionalParameterTypes = null) : ILGeneratorInstruction;