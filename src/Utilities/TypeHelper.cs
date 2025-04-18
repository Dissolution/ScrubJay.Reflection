#if NETFRAMEWORK || NETSTANDARD2_0
using Polyfills;
#endif

namespace ScrubJay.Reflection.Utilities;

[PublicAPI]
public static class TypeHelper
{
    private static readonly ConcurrentTypeMap<bool> _isRefCache = [];

#if NETFRAMEWORK || NETSTANDARD2_0
    // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/unmanaged-types

    private static bool DetermineIsRef(Type type)
    {
        if (type.IsEnum || type.IsPrimitive || type.IsPointer)
            return false;

        if (!type.IsValueType)
            return true;

        var fields = Reflect(type).Instance.Fields;
        foreach (var field in fields)
        {
            if (IsReferenceOrContainsReferences(field.FieldType))
                return true;
        }

        return false;
    }

    private static bool DetermineIsRef<T>(Type type) => DetermineIsRef(type);

#else
    private static bool DetermineIsRef(Type type)
    {
        return Reflect(typeof(RuntimeHelpers))
            .Public.Static.Methods
            .Named(nameof(RuntimeHelpers.IsReferenceOrContainsReferences))
            .GenericCount(1)
            .OneOrThrow("Could not find RuntimeHelpers.IsReferenceOrContainsReferences method")
            .MakeGenericMethod(type)
            .Invoke(null, null)
            .ThrowIfNot<bool>();
    }

    private static bool DetermineIsRef<T>(Type _)
    {
        return RuntimeHelpers.IsReferenceOrContainsReferences<T>();
    }
#endif

    public static bool IsReferenceOrContainsReferences(this Type? type)
    {
        if (type is null)
            return false;
        return _isRefCache.GetOrAdd(type, DetermineIsRef);
    }

    public static bool IsReferenceOrContainsReferences<T>()
    {
        return _isRefCache.GetOrAdd<T>(DetermineIsRef<T>);
    }

    public static bool IsUnmanaged(this Type? type)
    {
        if (type is null) return false;
        if (type == typeof(string) || Nullable.GetUnderlyingType(type) is not null)
            return false;
        return !IsReferenceOrContainsReferences(type);
    }

    public static bool IsUnmanaged<T>() => IsUnmanaged(typeof(T));
//    {
//        return !IsReferenceOrContainsReferences<T>();
//    }


    public static HashSet<Type> GetAllTypes()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(static assembly => Result.TryInvoke(assembly.GetTypes).OkOr([]))
            .ToHashSet();
    }
}