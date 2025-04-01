using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class TypeExtensions
{
    public static ReferenceKind RefKind(this Type type)
    {
        if (type.IsByRef)
        {
            return ReferenceKind.Ref;
        }
        return ReferenceKind.Default;
    }

    public static ReferenceKind RefKind(this Type type, out Type nonRefType)
    {
        if (type.IsByRef)
        {
            nonRefType = type.GetElementType()!;
            Debug.Assert(nonRefType is not null);
            return ReferenceKind.Ref;
        }
        nonRefType = type;
        return ReferenceKind.Default;
    }

    public static MemberInfo[] AllMembers(this Type? type)
    {
        if (type is null)
            return [];
        return type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
    }

    public static bool Equals(this Type? type, Type? other, TypeMatch typeMatch)
    {
        if (typeMatch.HasFlags<TypeMatch>(TypeMatch.Exact) && type == other)
            return true;
        if (typeMatch.HasFlags<TypeMatch>(TypeMatch.Implements) && type.Implements(other))
            return true;
        if (typeMatch.HasFlags<TypeMatch>(TypeMatch.ImplementedBy) && other.Implements(type))
            return true;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrVoid([NotNullWhen(false)] this Type? type) => type is null || type == typeof(void);


    private static readonly ConcurrentTypeMap<bool> _isUnmanagedCache = [];

    private static bool DetermineIfIsUnmanaged(Type type)
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        if (!type.IsValueType)
            return false;

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return fields.All(static field => field.FieldType.IsUnmanaged());
#else
        return !(typeof(RuntimeHelpers)
            .GetMethod(nameof(RuntimeHelpers.IsReferenceOrContainsReferences))
            .ThrowIfNull()
            .MakeGenericMethod(type)
            .Invoke(null, null)
            .ThrowIfNot<bool>());
#endif
    }
    
    private static bool DetermineIfIsUnmanaged<T>()
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        var type = typeof(T);
        if (!type.IsValueType)
            return false;

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return fields.All(static field => field.FieldType.IsUnmanaged());
#else
        return !RuntimeHelpers.IsReferenceOrContainsReferences<T>();
#endif
    }

    public static bool IsUnmanaged(this Type type)
    {
        return _isUnmanagedCache.GetOrAdd(type, DetermineIfIsUnmanaged);
    }

    public static bool IsUnmanaged<T>()
    {
        return _isUnmanagedCache.GetOrAdd<T>(DetermineIfIsUnmanaged<T>);
    }

    public static Type[]? NullIfNone(this Type[]? types)
    {
        if (types is null || types.Length == 0)
            return null;
        return types;
    }
}
