using Dunet;

namespace ScrubJay.Reflection.Runtime.Emission;

[Union]
public partial record class GenInstruction : Instruction
{
    public partial record class BeginCatchBlock(Type ExceptionType);
    public partial record class BeginExceptFilterBlock;
    public partial record class BeginExceptionBlock(EmitterLabel Label);
    public partial record class EndExceptionBlock;
    public partial record class BeginFaultBlock;
    public partial record class BeginFinallyBlock;
    public partial record class BeginScope;
    public partial record class EndScope;
    public partial record class UsingNamespace(string Namespace);
    public partial record class DeclareLocal(EmitterLocal Local);
    public partial record class DefineLabel(EmitterLabel Label);
    public partial record class MarkLabel(EmitterLabel Label);
    public partial record class CallVarargs(MethodInfo Method, Type[]? OptionalParameterTypes);
    public partial record class CallManaged(CallingConventions Conventions, Type? ReturnType, Type[]? ParameterTypes, Type[]? OptionalParameterTypes);
    public partial record class CallUnmanaged(CallingConvention Convention, Type? ReturnType, Type[]? ParameterTypes);
    public partial record class WriteLine(object? Value);
    public partial record class ThrowException(Type ExceptionType);
}

