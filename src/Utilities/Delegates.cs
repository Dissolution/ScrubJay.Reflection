namespace ScrubJay.Reflection.Utilities;

public static class Delegates
{
    public static MethodInfo? GetInvokeMethod(Type? delegateType)
    {
        return delegateType?.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
    }

    public static MethodInfo GetInvokeMethod<TDelegate>()
        where TDelegate : Delegate
    {
        return GetInvokeMethod(typeof(TDelegate))!;
    }
}