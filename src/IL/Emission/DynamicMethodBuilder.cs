using ScrubJay.Reflection.Utilities;
using ScrubJay.Reflection.Validation;

namespace ScrubJay.Reflection.IL.Emission;

public class DynamicMethodBuilder : ILMethod
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
    internal DynamicMethodBuilder(Type delegateType, string? name = null)
    {
        MemberAssert.IsDelegateType(delegateType);
        this.DelegateType = delegateType;
        this.DelegateInvokeMethod = delegateType.InvokeMethod().SomeOrThrow();
        _dynamicMethod = RuntimeBuilder.CreateDynamicMethod(DelegateInvokeMethod);
        
        this.Name = name ?? delegateType.Name;
        this.Parameters = _dynamicMethod.GetParameters();
        this.ReturnParameter = _dynamicMethod.ReturnParameter;
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

public class DynamicMethodBuilder<D> : DynamicMethodBuilder
    where D : Delegate
{
    [SetsRequiredMembers]
    public DynamicMethodBuilder(string? name = null) 
        : base(typeof(D), name)
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