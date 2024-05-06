using ScrubJay.Extensions;
using ScrubJay.Reflection.Info;
using ScrubJay.Reflection.Runtime.Emission;

namespace ScrubJay.Reflection.Runtime;

public class DelegateBuilder
{
    protected readonly DynamicMethod _dynamicMethod;
    protected readonly Type _delegateType;
    protected ILGenerator? _ilGenerator;
    protected ILEmitter? _ilEmitter;
    
    public ILGenerator ILGenerator
    {
        get => _ilGenerator ??= _dynamicMethod.GetILGenerator();
    }

    public ILEmitter Emitter
    {
        get => _ilEmitter ??= new(ILGenerator);
    }
    
    public DelegateBuilder(Type delegateType)
    {
        if (!delegateType.Implements<Delegate>())
            throw new ArgumentException("Not a valid Delegate Type", nameof(delegateType));
        _delegateType = delegateType;
        _dynamicMethod = RuntimeBuilder.CreateDynamicMethod(MethodSignature.For(delegateType));
    }

    public Delegate CreateDelegate()
    {
        return _dynamicMethod.CreateDelegate(_delegateType);
    }
}

public class DelegateBuilder<TDelegate> : DelegateBuilder
    where TDelegate : Delegate
{
    public DelegateBuilder() : base(typeof(TDelegate))
    {
        
    }

    public new TDelegate CreateDelegate()
    {
        return _dynamicMethod.CreateDelegate<TDelegate>();
    }
}