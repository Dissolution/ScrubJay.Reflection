namespace ScrubJay.Reflection.IL.Instructions;

/// <summary>
/// Specifies a particular non-Emit method on an <see cref="ILGenerator"/>
/// </summary>
[PublicAPI]
public enum ILGeneratorMethod
{
    BeginCatchBlock,
    BeginExceptFilterBlock,
    BeginExceptionBlock,
    EndExceptionBlock,
    BeginFaultBlock,
    BeginFinallyBlock,
    ThrowException,
    UsingNamespace,
    BeginScope,
    EndScope,
    CallManaged,
    CallUnmanaged,
    CallVarargs,
    DeclareLocal,
    DefineLabel,
    MarkLabel,
    WriteLine,
}