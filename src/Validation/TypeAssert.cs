namespace ScrubJay.Reflection.Validation;

[PublicAPI]
public static class TypeAssert
{
    public static void IsDelegate([AllowNull, NotNull] Type? type, 
        [CallerArgumentExpression(nameof(type))] string? typeName = null)
    {
        if (type is null)
            throw new ArgumentNullException(typeName);
        if (!type.Implements<Delegate>())
            throw new ArgumentOutOfRangeException(typeName, type, "Type is not a Delegate type");
    }

    public static void IsException([AllowNull, NotNull] Type? type,
        [CallerArgumentExpression(nameof(type))]
        string? typeName = null)
    {
        if (type is null)
            throw new ArgumentNullException(typeName);
        if (!type.Implements<Exception>())
            throw new ArgumentOutOfRangeException(typeName, type, "Type is not an Exception type");
    }
    
    public static void IsValue([AllowNull, NotNull] Type? type,
        [CallerArgumentExpression(nameof(type))]
        string? typeName = null)
    {
        if (type is null)
            throw new ArgumentNullException(typeName);
        if (!type.IsValueType)
            throw new ArgumentOutOfRangeException(typeName, type, "Type is not a ValueType");
    }
    
    public static void IsClassOrInterface([AllowNull, NotNull] Type? type,
        [CallerArgumentExpression(nameof(type))]
        string? typeName = null)
    {
        if (type is null)
            throw new ArgumentNullException(typeName);
        if (!type.IsClass && !type.IsInterface)
            throw new ArgumentOutOfRangeException(typeName, type, "Type is not a class or interface type");
    }
    
    public static void IsInstance([AllowNull, NotNull] Type? type,
        [CallerArgumentExpression(nameof(type))]
        string? typeName = null)
    {
        if (type is null)
            throw new ArgumentNullException(typeName);
        if (type.IsStatic())
            throw new ArgumentOutOfRangeException(typeName, type, "Type is not an instance type");
    }
    
    public static void IsStatic([AllowNull, NotNull] Type? type,
        [CallerArgumentExpression(nameof(type))]
        string? typeName = null)
    {
        if (type is null)
            throw new ArgumentNullException(typeName);
        if (!type.IsStatic())
            throw new ArgumentOutOfRangeException(typeName, type, "Type is not a static type");
    }
}