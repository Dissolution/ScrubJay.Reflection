namespace ScrubJay.Reflection.Utilities;

[PublicAPI]
public static class DelegateHelper
{
    public static Option<MethodInfo> InvokeMethod(this Type? delegateType)
    {
        if (delegateType is null)
            return None();
        return Option.NotNull(delegateType
            .GetMethod("Invoke", BF.Public | BF.Instance));
    }

    public static MethodInfo InvokeMethod<TDelegate>()
        where TDelegate : Delegate
    {
        return typeof(TDelegate)
            .GetMethod("Invoke", BF.Public | BF.Instance)
            .ThrowIfNull();
    }

    public static string NameOf<TDelegate>(this TDelegate del)
        where TDelegate : Delegate
        => typeof(TDelegate).NameOf();
}