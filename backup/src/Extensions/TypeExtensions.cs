namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class TypeExtensions
{
    public static void Deconstruct(this Type? type, out TRK refKind, [NotNullIfNotNull(nameof(type))] out Type? underlyingType)
    {
        if (type is null)
        {
            refKind = TRK.Default;
            underlyingType = null;
        }
        else if (type.IsByRef)
        {
            refKind = TRK.Ref;
            underlyingType = type.GetElementType();
            Debug.Assert(underlyingType is not null);
        }
        else
        {
            refKind = TRK.Default;
            underlyingType = type;
        }
    }

    public static TRK TypeRefKind(this Type? type)
    {
        if (type is null || !type.IsByRef)
            return TRK.Default;
        return TRK.Ref;
    }

    /// <summary>
    /// Is this <see cref="Type"/> <c>null</c> or <c>void</c>?
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrVoid([NotNullWhen(false)] this Type? type) => type is null || type == typeof(void);

    public static bool IsVoidLike([AllowNull, NotNullWhen(false)] this Type? type)
    {
        if (type is null)
            return true;
        if (type == typeof(void))
            return true;
        if (type == typeof(None))
            return true;
        if (type == typeof(Unit))
            return true;
        return false;
    }
    
    
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
    
    public static Type VoidIfNull(this Type? type)
    {
        if (type is null)
            return typeof(void);
        return type;
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
    
    #region ValueTuple
    private static readonly HashSet<Type> _valueTupleTypes =
    [
        typeof(ValueTuple),
        typeof(ValueTuple<>),
        typeof(ValueTuple<,>),
        typeof(ValueTuple<,,>),
        typeof(ValueTuple<,,,>),
        typeof(ValueTuple<,,,,>),
        typeof(ValueTuple<,,,,,>),
        typeof(ValueTuple<,,,,,,>),
        typeof(ValueTuple<,,,,,,,>),
    ];

    public static bool IsValueTuple(this object? obj) => IsValueTuple(obj?.GetType());

    public static bool IsValueTuple(this Type? type)
    {
        return type is not null &&
            type.IsGenericType &&
            _valueTupleTypes.Contains(type.GetGenericTypeDefinition());
    }
    #endregion
    
    public static object? GetDefault(this Type type)
    {
        if (type.IsValueType)
            return Activator.CreateInstance(type);
        return null;
    }

    public static object GetUninitialized(this Type type)
    {
        return RuntimeBuilder.GetUninitialized(type);
    }
}