using ScrubJay.Reflection.Comparison;

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

        var fields = Reflect(type).Instance.Fields();
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
            .Public.Static.Methods()
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
    
    public static IReadOnlyList<Type> GetImplementedTypes(Type? type)
    {
        if (type is null)
            return [];
        
        var types = new HashSet<Type>();
        
        // add all interfaces
        type.GetInterfaces().Consume(it => types.Add(it));
       
        // Add base types
        type = type.BaseType;
        while (type is not null)
        {
            types.Add(type);
            type = type.BaseType;
        }
       
        // return sorted!
        return types
            .OrderByDescending(static t => t, TypeComplexityComparer.Default)
            .ToList();
    }
    

    private static Option<int> CanCastValueTypeTo(Type valueType, Type targetType)
    {
        Debug.Assert(valueType.IsValueType);    // already verified
        Debug.Assert(valueType != targetType);  // already verified
        if (targetType.IsValueType)
        {
            // TODO: implicit unmanaged type conversions (byte -> int)
            return None();
        }
        else if (targetType == typeof(object))
        {
            return Some(100);
        }
        else if (targetType.IsInterface)
        {
            foreach (Type interfaceType in valueType.GetInterfaces())
            {
                if (interfaceType == targetType)
                    return Some(9 - interfaceType.GetGenericArguments().Length);
            }
            return None();
        }
        else
        {
            return None();
        }
    }

    private static int GetObjectCastExactness() => 1_000_000;
    private static int GetInterfaceCastExactness() => 0_001_000;
    private static int GetClassCastExactNess() => 0_000_001;
    
    public static Option<int> CanCast(Type sourceType, Type targetType)
    {
        if (sourceType == targetType)
            return Some(0); // baseline exactness

        if (targetType.IsGenericTypeDefinition)
            return None(); // we cannot cast to a definition
        
        if (sourceType.IsValueType)
        {
            return CanCastValueTypeTo(sourceType, targetType);
        }
        
        if (sourceType == typeof(object))
            return Some(100);
        
        if (targetType.IsInterface)
        {
            foreach (Type interfaceType in sourceType.GetInterfaces())
            {
                if (interfaceType == targetType)
                    return Some(9 - interfaceType.GetGenericArguments().Length);
            }
            return None();
        }
        for (Type? baseType = sourceType.BaseType; baseType != null; baseType = baseType.BaseType)
        {
            if (baseType == targetType)
                return Some(1);
        }
        return None();
    }
}