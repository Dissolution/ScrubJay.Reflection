using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class TypeExtensions
{
    public static void Deconstruct(this Type? type, out TypeRefKind refKind, [NotNullIfNotNull(nameof(type))] out Type? underlyingType)
    {
        if (type is null)
        {
            refKind = TypeRefKind.Default;
            underlyingType = null;
        }
        else if (type.IsByRef)
        {
            refKind = TypeRefKind.Ref;
            underlyingType = type.GetElementType();
            Debug.Assert(underlyingType is not null);
        }
        else
        {
            refKind = TypeRefKind.Default;
            underlyingType = type;
        }
    }

    /// <summary>
    /// Is this <see cref="Type"/> <c>null</c> or <c>void</c>?
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrVoid([NotNullWhen(false)] this Type? type) => type is null || type == typeof(void);

    /// <summary>
    /// If <paramref name="types"/> is <c>null</c> or empty (Length == 0), return <c>null</c>;<br/>
    /// otherwise, returns <paramref name="types"/>
    /// </summary>
    public static Type[]? NullIfNone(this Type[]? types)
    {
        if (types is null || types.Length == 0)
            return null;
        return types;
    }

    public static Type? NullIfVoid(this Type? type)
    {
        if (type is null || type == typeof(void))
            return null;
        return type;
    }

    /// <summary>
    /// Gets all <see cref="MemberInfo"/>s that belongs to this <see cref="Type"/>
    /// </summary>
    public static IReadOnlyList<MemberInfo> AllMembers(this Type? type)
    {
        if (type is null)
            return [];
        return type.GetMembers(BF.Public | BF.NonPublic | BF.Instance | BF.Static | BF.FlattenHierarchy);
    }
//
//    public static bool Equals(this Type? type, Type? other, TypeMatch typeMatch)
//    {
//        if (typeMatch.HasFlags<TypeMatch>(TypeMatch.Exact) && type == other)
//            return true;
//        if (typeMatch.HasFlags<TypeMatch>(TypeMatch.Implements) && type.Implements(other))
//            return true;
//        if (typeMatch.HasFlags<TypeMatch>(TypeMatch.ImplementedBy) && other.Implements(type))
//            return true;
//        return false;
//    }


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
        return DetermineIfIsUnmanaged(typeof(T));
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

#region Visibility
    private static bool IsPublic(Type type)
    {
        // public types are visible
        return type.IsVisible &&
            (type.IsPublic || (type.IsNested && type.IsNestedPublic));
    }

    private static bool IsInternal(Type type)
    {
        return type.IsNotPublic ||
            (type.IsNested && (type.IsNestedAssembly || type.IsNestedFamORAssem || type.IsNestedFamANDAssem));
    }

    // only nested types can be declared "protected"
    private static bool IsProtected(Type type)
    {
        return type.IsNested && (type.IsNestedFamily || type.IsNestedFamORAssem || type.IsNestedFamANDAssem);
    }

    // only nested types can be declared "private"
    private static bool IsPrivate(Type type)
    {
        return type.IsNested && type.IsNestedPrivate;
    }
    
    public static Viz Visibility(this Type? type)
    {
        var visibility = Viz.None;
        if (type is null) 
            return visibility;
        if (type.IsStatic())
        {
            visibility |= Viz.Static;
        }
        else
        {
            visibility |= Viz.Instance;
        }
        if (IsPublic(type))
            visibility |= Viz.Public;
        if (IsInternal(type))
            visibility |= Viz.Internal;
        if (IsProtected(type))
            visibility |= Viz.Protected;
        if (IsPrivate(type))
            visibility |= Viz.Private;
        return visibility;
    }
#endregion
}