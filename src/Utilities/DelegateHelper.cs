namespace ScrubJay.Reflection.Utilities;

[PublicAPI]
public static class DelegateHelper
{
    [return: NotNullIfNotNull(nameof(delegateType))]
    public static MethodInfo? GetInvokeMethod(Type? delegateType)
    {
        if (delegateType is null)
            return null;
        return delegateType.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance)
            .ThrowIfNull();
    }

    public static MethodInfo GetInvokeMethod<TDelegate>()
        where TDelegate : Delegate
    {
        return typeof(TDelegate).GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance).ThrowIfNull();
    }

    public static string Dump<TDelegate>()
        where TDelegate : Delegate
        => typeof(TDelegate).NameOf();
}