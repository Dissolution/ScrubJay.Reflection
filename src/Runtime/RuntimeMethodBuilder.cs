using ScrubJay.Reflection.Info;
using ScrubJay.Reflection.Runtime.Emission;

namespace ScrubJay.Reflection.Runtime;

public class DelegateBuilder
{
    protected readonly DynamicMethod _dynamicMethod;
    protected ILGenerator? _ilGenerator;
    protected IBasicEmitter? _ilEmitter;
    
    public DelegateSignature Signature { get; }
    
    public ILGenerator ILGenerator
    {
        get => _ilGenerator ??= _dynamicMethod.GetILGenerator();
    }

    public IBasicEmitter BasicEmitter
    {
        get => _ilEmitter ?? throw new NotImplementedException();
    }

    public DelegateBuilder(DelegateSignature signature)
    {
        this.Signature = signature;
        _dynamicMethod = RuntimeBuilder.CreateDynamicMethod(signature);
    }

    public Delegate CreateDelegate()
    {
        return _dynamicMethod.CreateDelegate(this.Signature.GetOrCreateDelegateType());
    }
}

public class DelegateBuilder<TDelegate> : DelegateBuilder
    where TDelegate : Delegate
{
    public DelegateBuilder() : base(DelegateSignature.For<TDelegate>())
    {

    }

    public new TDelegate CreateDelegate()
    {
#if NET6_0_OR_GREATER
        return _dynamicMethod.CreateDelegate<TDelegate>();
#else
        return (TDelegate)_dynamicMethod.CreateDelegate(typeof(TDelegate));
#endif
    }
}