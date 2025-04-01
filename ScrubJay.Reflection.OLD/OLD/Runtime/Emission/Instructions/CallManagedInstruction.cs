using System.Reflection;

namespace ScrubJay.Reflection.OLD.OLD.Runtime.Emission.Instructions;

public record CallManagedInstruction(CallingConventions Conventions, Type? ReturnType, Type[]? ParameterTypes, Type[]? OptionalParameterTypes = null) : ILGeneratorInstruction;