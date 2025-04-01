using ScrubJay.Collections;
using ScrubJay.Fluent;
using ScrubJay.Sigil.Utilities;
using ScrubJay.Text;
using ScrubJay.Validation;

namespace ScrubJay.Sigil.Emission.Fluent;

internal static class FluentEmit
{
    internal static readonly ModuleBuilder _module;

    static FluentEmit()
    {
        AssemblyName assemblyName = new AssemblyName("ScrubJay.Sigil.FluentEmit.DynamicAssembly");
#if NETFRAMEWORK
        var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#else
        var assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#endif
        _module = assembly.DefineDynamicModule("DynamicModule");
    }
}

public abstract class FluentEmit<TSelf> : FluentBuilder<TSelf>
    where TSelf : FluentEmit<TSelf>
{
    public CallingConventions CallingConventions { get; }
    internal TypeOnStack ReturnType { get; }
    public Type[] ParameterTypes { get; }

    protected FluentEmit(CallingConventions callConvention, Type returnType, Type[] parameterTypes)
    {
        this.CallingConventions = callConvention;
        this.ReturnType = TypeOnStack.Get(returnType);
        this.ParameterTypes = parameterTypes;
    }
}

public class BufferedInstruction
{
    public Action<ILGenerator> Generate { get; }
    public Option<OpCode> OpCode { get; }
    public Option<ILGeneratorMethod> ILGeneratorMethod { get; }
    public Values<object> Arguments { get; }
    public int Size { get; }

    public override string ToString() => TextBuilder.New
        .If(OpCode, static (tb, opcode) => tb.Append(opcode.Name))
        .If(ILGeneratorMethod, static (tb, method) => tb.Append(method))
        .If(Arguments.Count > 0, tb => tb.Append(": ").Append(Arguments))
        .ToStringAndDispose();
}

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
