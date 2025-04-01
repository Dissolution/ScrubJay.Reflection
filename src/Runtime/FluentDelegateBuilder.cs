using ScrubJay.Fluent;
using ScrubJay.Reflection.Runtime.Emission;

namespace ScrubJay.Reflection.Runtime;

public sealed class DelegateBuilder : FluentDelegateBuilder<DelegateBuilder>
{
    public DelegateBuilder(Type delegateType, DynamicMethod dynamicMethod) : base(delegateType, dynamicMethod)
    {
    }
}

public abstract class FluentDelegateBuilder<TB> : FluentBuilder<TB>
    where TB : FluentDelegateBuilder<TB>
{
    private ILGenerator? _ilGenerator;
    private FluentILEmitter? _emitter;

    public Type DelegateType { get; }
    public DynamicMethod DynamicMethod { get; }
    public ILGenerator ILGenerator => _ilGenerator ??= DynamicMethod.GetILGenerator();
    public FluentILEmitter Emitter => _emitter ??= new(ILGenerator);

    protected FluentDelegateBuilder(Type delegateType, DynamicMethod dynamicMethod)
    {
        this.DelegateType = delegateType;
        this.DynamicMethod = dynamicMethod;
    }

    public Result<Delegate> TryBuild()
    {
        try
        {
            return this.DynamicMethod.CreateDelegate(DelegateType);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}

public sealed class DelegateBuilder<TD> : FluentDelegateBuilder<DelegateBuilder<TD>, TD>
    where TD : Delegate
{
    public DelegateBuilder(DynamicMethod dynamicMethod) : base(dynamicMethod)
    {
    }
}

public abstract class FluentDelegateBuilder<TB, TD> : FluentDelegateBuilder<TB>
    where TB : FluentDelegateBuilder<TB, TD>
    where TD : Delegate
{
    protected FluentDelegateBuilder(DynamicMethod dynamicMethod)
        : base(typeof(TD), dynamicMethod)
    {
    }

    public TD Build() => TryBuild().OkOrThrow();

    public new Result<TD> TryBuild()
    {
        try
        {
            return this.DynamicMethod.CreateDelegate<TD>();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}
