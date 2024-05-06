using ScrubJay.Extensions;
using ScrubJay.Reflection.Extensions;
using ScrubJay.Reflection.Utilities;

namespace ScrubJay.Reflection.Info;

public record class MethodSignature : DelegateSignature
{
    public static MethodSignature For(MethodBase method)
    {
        return new MethodSignature
        {
            Name = method.Name,
            ParameterTypes = method.GetParameterTypes(),
            ReturnType = method.ReturnType(),
        };
    }
    
    public static MethodSignature For<TDelegate>()
        where TDelegate : Delegate
    {
        return For(Delegates.GetInvokeMethod<TDelegate>());
    }

    public static MethodSignature For(Type delegateType)
    {
        if (!delegateType.Implements<Delegate>())
            throw new ArgumentException("Not a valid Delegate Type", nameof(delegateType));
        return For(Delegates.GetInvokeMethod(delegateType));
    }

    public static MethodSignature For(Delegate @delegate)
    {
        return For(@delegate.Method);
    }

    public required string Name { get; init; } = "Invoke";
}