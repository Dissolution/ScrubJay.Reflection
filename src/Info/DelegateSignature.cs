using ScrubJay.Reflection.Utilities;

namespace ScrubJay.Reflection.Info;

public record class DelegateSignature
{
    public static DelegateSignature For<TDelegate>()
        where TDelegate : Delegate
    {
        return MethodSignature.For(Delegates.GetInvokeMethod<TDelegate>());
    }
    
    public required Type[] ParameterTypes { get; init; } = Type.EmptyTypes;
    public required Type ReturnType { get; init; } = typeof(void);
}