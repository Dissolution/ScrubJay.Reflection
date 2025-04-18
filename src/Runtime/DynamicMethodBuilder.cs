using ScrubJay.Reflection.IL.Emission;
using ScrubJay.Reflection.IL.Instructions;
using ScrubJay.Reflection.Utilities;
using ScrubJay.Reflection.Validation;

namespace ScrubJay.Reflection.Runtime;

public class DynamicMethodBuilder
{
    protected readonly DynamicMethod _dynamicMethod;
    
    private Type? _returnType;
    private ParameterInfo[]? _parameters;
    private Type[]? _parameterTypes;
    private ILGenerator? _ilGenerator;
    private Emitter? _emitter;

    public DynamicMethod DynamicMethod => _dynamicMethod;
    
    public Type DelegateType { get; }
    
    public MethodInfo DelegateInvokeMethod { get; }

    public Type ReturnType => _returnType ??= _dynamicMethod.ReturnType;

    public ParameterInfo[] Parameters => _parameters ??= _dynamicMethod.GetParameters();

    public Type[] ParameterTypes => _parameterTypes ??= Parameters.ConvertAll(static p => p.ParameterType);

    public ILGenerator ILGenerator => _ilGenerator ??= _dynamicMethod.GetILGenerator();

    public Emitter Emitter => _emitter ??= new(ILGenerator);

    public string? Name { get; }
    
    public DynamicMethodBuilder(Type delegateType, string? name = null)
    {
        TypeAssert.IsDelegate(delegateType);
        this.DelegateType = delegateType;
        this.DelegateInvokeMethod = delegateType.InvokeMethod().SomeOrThrow();
        _dynamicMethod = RuntimeBuilder.CreateDynamicMethod(DelegateInvokeMethod);
        this.Name = name ?? delegateType.Name;
    }
    
    public Result<Delegate> TryCreateDelegate()
    {
        try
        {
            return Ok(_dynamicMethod.CreateDelegate(DelegateType));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public override string ToString()
    {
        return TextBuilder.New
            .AppendIf(_dynamicMethod.IsStatic, "static ")
            .AppendType(_dynamicMethod.OwnerType())
            .Append('.')
            .AppendNameAndGenericTypes(_dynamicMethod.Name, DelegateInvokeMethod.GetGenericArguments())
            .AppendLine('(')
            .Delimit(static tb => tb.Append(',').NewLine(),
                Parameters,
                static (tb, param) => tb.Append('[').Append(param.Position).Append("] ").AppendParameter(param))
            .NewLine()
            .Append(") => ").AppendType(ReturnType).NewLine()
            .AppendLine("-- Locals")
            .Enumerate(Emitter.Locals, (tb, local) => tb.Render(local).NewLine())
            .AppendLine("-- CIL")
            .Enumerate(Emitter.Instructions, (tb, instr) =>
            {
                if (instr is ILGeneratorDeclareLocalInstruction or ILGeneratorDefineLabelInstruction)
                    return;
                else if (instr is ILGeneratorMarkLabelInstruction markLabel)
                    tb.Invoke(markLabel.Label.RenderTo).NewLine();
                else
                    tb.Invoke(instr.RenderTo).NewLine();
            })
            .ToStringAndDispose();
    }
}

public class DynamicMethodBuilder<D> : DynamicMethodBuilder
    where D : Delegate
{
    public DynamicMethodBuilder() : base(typeof(D))
    {
        
    }
    
    public new Result<D> TryCreateDelegate()
    {
        try
        {
            return Ok(_dynamicMethod.CreateDelegate<D>());
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}