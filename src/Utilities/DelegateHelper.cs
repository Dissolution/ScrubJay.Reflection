namespace ScrubJay.Reflection.Utilities;

[PublicAPI]
public static class DelegateHelper
{
    public static Option<MethodInfo> GetInvokeMethod(this Type? delegateType)
    {
        if (delegateType is null)
            return None();
        return Option.NotNull(delegateType
            .GetMethod("Invoke", BF.Public | BF.Instance));
    }

    public static MethodInfo GetInvokeMethod<TDelegate>()
        where TDelegate : Delegate
    {
        return typeof(TDelegate)
            .GetMethod("Invoke", BF.Public | BF.Instance)
            .ThrowIfNull();
    }

    public static string Render<TDelegate>(this TDelegate del)
        where TDelegate : Delegate
        => typeof(TDelegate).Render();

    public static Type CreateDelegateType(Type[] parameterTypes, Type returnType)
    {
        int parameterCount = parameterTypes.Length;
        
        if (returnType == typeof(void))
        {
            // action
            return parameterCount switch
            {
                0 => typeof(Action),
                1 => typeof(Action<>).MakeGenericType(parameterTypes),
                2 => typeof(Action<,>).MakeGenericType(parameterTypes),
                3 => typeof(Action<,,>).MakeGenericType(parameterTypes),
                4 => typeof(Action<,,,>).MakeGenericType(parameterTypes),
                5 => typeof(Action<,,,,>).MakeGenericType(parameterTypes),
                6 => typeof(Action<,,,,,>).MakeGenericType(parameterTypes),
                7 => typeof(Action<,,,,,,>).MakeGenericType(parameterTypes),
                8 => typeof(Action<,,,,,,,>).MakeGenericType(parameterTypes),
                9 => typeof(Action<,,,,,,,,>).MakeGenericType(parameterTypes),
                10 => typeof(Action<,,,,,,,,,>).MakeGenericType(parameterTypes),
                11 => typeof(Action<,,,,,,,,,,>).MakeGenericType(parameterTypes),
                12 => typeof(Action<,,,,,,,,,,,>).MakeGenericType(parameterTypes),
                13 => typeof(Action<,,,,,,,,,,,,>).MakeGenericType(parameterTypes),
                14 => typeof(Action<,,,,,,,,,,,,,>).MakeGenericType(parameterTypes),
                15 => typeof(Action<,,,,,,,,,,,,,,>).MakeGenericType(parameterTypes),
                16 => typeof(Action<,,,,,,,,,,,,,,,>).MakeGenericType(parameterTypes),
                _ => throw new ArgumentException(null, nameof(parameterTypes)),
            };
        }
        else
        {
            Type[] pTypes = [..parameterTypes, returnType];
            
            // func
            return parameterCount switch
            {
                0 => typeof(Func<>).MakeGenericType(pTypes),
                1 => typeof(Func<,>).MakeGenericType(pTypes),
                2 => typeof(Func<,,>).MakeGenericType(pTypes),
                3 => typeof(Func<,,,>).MakeGenericType(pTypes),
                4 => typeof(Func<,,,,>).MakeGenericType(pTypes),
                5 => typeof(Func<,,,,,>).MakeGenericType(pTypes),
                6 => typeof(Func<,,,,,,>).MakeGenericType(pTypes),
                7 => typeof(Func<,,,,,,,>).MakeGenericType(pTypes),
                8 => typeof(Func<,,,,,,,,>).MakeGenericType(pTypes),
                9 => typeof(Func<,,,,,,,,,>).MakeGenericType(pTypes),
                10 => typeof(Func<,,,,,,,,,,>).MakeGenericType(pTypes),
                11 => typeof(Func<,,,,,,,,,,,>).MakeGenericType(pTypes),
                12 => typeof(Func<,,,,,,,,,,,,>).MakeGenericType(pTypes),
                13 => typeof(Func<,,,,,,,,,,,,,>).MakeGenericType(pTypes),
                14 => typeof(Func<,,,,,,,,,,,,,,>).MakeGenericType(pTypes),
                15 => typeof(Func<,,,,,,,,,,,,,,,>).MakeGenericType(pTypes),
                16 => typeof(Func<,,,,,,,,,,,,,,,,>).MakeGenericType(pTypes),
                _ => throw new ArgumentException(null, nameof(parameterTypes)),
            };
        }
    }
}