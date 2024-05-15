using Dunet;

namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

[Union]
public partial record ILGeneratorInstruction : Instruction
{
    partial record BeginCatchBlock(Type ExceptionType);
    partial record BeginExceptFilterBlock;
    partial record BeginExceptionBlock(EmitterLabel Label);
    partial record EndExceptionBlock;
    partial record BeginFaultBlock;
    partial record BeginFinallyBlock;
    partial record BeginScope;
    partial record EndScope;
    partial record UsingNamespace(string Namespace);
    partial record DeclareLocal(EmitterLocal Local);
    partial record DefineLabel(EmitterLabel Label);
    partial record MarkLabel(EmitterLabel Label);
    partial record CallVarargs(MethodInfo Method, Type[]? OptionalParameterTypes = null);
    partial record CallManaged(CallingConventions Conventions, Type? ReturnType, Type[]? ParameterTypes, Type[]? OptionalParameterTypes = null);
    partial record CallUnmanaged(CallingConvention Convention, Type? ReturnType, Type[]? ParameterTypes);
    partial record WriteLine(object? Value);
    partial record ThrowException(Type ExceptionType);

    public override int Size => 0;
}