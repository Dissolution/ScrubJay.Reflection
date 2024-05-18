using ScrubJay.Reflection.Runtime.Naming;

namespace ScrubJay.Reflection.Utilities;

public static class Delegates
{
    [return: NotNullIfNotNull(nameof(delegateType))]
    public static MethodInfo? GetInvokeMethod(Type? delegateType)
    {
        return delegateType?.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
    }

    public static MethodInfo GetInvokeMethod<TDelegate>()
        where TDelegate : Delegate
    {
        return GetInvokeMethod(typeof(TDelegate))!;
    }

    public static string Dump<TDelegate>()
        where TDelegate : Delegate
        => typeof(TDelegate).Dump();
}