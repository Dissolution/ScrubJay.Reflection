using ScrubJay.Reflection.Validation;

namespace ScrubJay.Reflection.IL.Emission;

public class DynamicILMethod : ILMethod
{
    protected readonly DynamicMethod _dynamicMethod;
    
    private ILGenerator? _ilGenerator;
    private Emitter? _emitter;

    public DynamicMethod DynamicMethod => _dynamicMethod;
    
    public Type DelegateType { get; }
    
    public MethodInfo DelegateInvokeMethod { get; }

    public ILGenerator ILGenerator => _ilGenerator ??= _dynamicMethod.GetILGenerator();

    public Emitter Emitter => _emitter ??= new(this, ILGenerator);
    
    [SetsRequiredMembers]
    internal DynamicILMethod(Type delegateType, string? name = null)
    {
        MemberAssert.IsDelegateType(delegateType);
        this.DelegateType = delegateType;
        this.DelegateInvokeMethod = delegateType.InvokeMethod().SomeOrThrow();
        _dynamicMethod = RuntimeBuilder.CreateDynamicMethod(DelegateInvokeMethod);
        
        this.Name = name ?? delegateType.Name;
        this.Parameters = _dynamicMethod.GetParameters();
        this.ReturnParameter = _dynamicMethod.ReturnParameter;
    }

    [SetsRequiredMembers]
    internal DynamicILMethod(DelegateInfo info, string? name = null)
    {
        this.DelegateType = info.DelegateType;
        this.DelegateInvokeMethod = DelegateType.InvokeMethod().SomeOrThrow();
        _dynamicMethod = RuntimeBuilder.CreateDynamicMethod(info);

        this.Name = name ?? info.Name;
        this.Parameters = info.Parameters;
        this.ReturnParameter = info.ReturnParameter;
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
}

public class DynamicILMethod<D> : DynamicILMethod
    where D : Delegate
{
    [SetsRequiredMembers]
    public DynamicILMethod(string? name = null) 
        : base(DelegateInfo.New<D>(), name)
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