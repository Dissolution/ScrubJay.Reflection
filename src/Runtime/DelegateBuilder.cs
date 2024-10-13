using ScrubJay.Reflection.Info;
using ScrubJay.Reflection.Runtime.Emission.Emitters;

namespace ScrubJay.Reflection.Runtime;

public class DelegateBuilder
{
    protected readonly DynamicMethod _dynamicMethod;
    protected ILGenerator? _ilGenerator;
    protected ICleanEmitter? _ilEmitter;
    
    public DelegateSignature Signature { get; }
    
    public ILGenerator ILGenerator
    {
        get => _ilGenerator ??= _dynamicMethod.GetILGenerator();
    }

    public ICleanEmitter Emitter
    {
        get => _ilEmitter ?? Emission.Emitters.Emitter.GetCleanEmitter(ILGenerator);
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