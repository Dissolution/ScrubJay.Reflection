using Dunet;

namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public abstract record ILGeneratorInstruction : Instruction
{
    public override int Size => 0;
}

public record BeginCatchBlockInstruction(Type ExceptionType) : ILGeneratorInstruction;
public record BeginExceptFilterBlockInstruction : ILGeneratorInstruction;
public record BeginExceptionBlockInstruction(EmitterLabel Label) : ILGeneratorInstruction;
public record EndExceptionBlockInstruction : ILGeneratorInstruction;
public record BeginFaultBlockInstruction : ILGeneratorInstruction;
public record BeginFinallyBlockInstruction : ILGeneratorInstruction;
public record BeginScopeInstruction : ILGeneratorInstruction;
public record EndScopeInstruction : ILGeneratorInstruction;
public record UsingNamespaceInstruction(string Namespace) : ILGeneratorInstruction;
public record DeclareLocalInstruction(EmitterLocal Local) : ILGeneratorInstruction;
public record DefineLabelInstruction(EmitterLabel Label) : ILGeneratorInstruction;
public record MarkLabelInstruction(EmitterLabel Label) : ILGeneratorInstruction;
public record CallVarargsInstruction(MethodInfo Method, Type[]? OptionalParameterTypes = null) : ILGeneratorInstruction;
public record CallManagedInstruction(CallingConventions Conventions, Type? ReturnType, Type[]? ParameterTypes, Type[]? OptionalParameterTypes = null) : ILGeneratorInstruction;
public record CallUnmanagedInstruction(CallingConvention Convention, Type? ReturnType, Type[]? ParameterTypes) : ILGeneratorInstruction;
public record WriteLineInstruction(object? Value) : ILGeneratorInstruction;
public record ThrowExceptionInstruction(Type ExceptionType) : ILGeneratorInstruction;