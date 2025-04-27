using ScrubJay.Reflection.Naming;

namespace ScrubJay.Reflection.Validation;

[PublicAPI]
public static class MemberAssert
{
    public static void IsDelegateType([AllowNull, NotNull] Type? type, 
        [CallerArgumentExpression(nameof(type))] string? typeName = null)
    {
        if (type is null)
            throw new ArgumentNullException(typeName);
        if (!type.Implements<Delegate>())
            throw new ArgumentOutOfRangeException(typeName, type, "Type is not a Delegate type");
    }

    public static void IsExceptionType([AllowNull, NotNull] Type? type,
        [CallerArgumentExpression(nameof(type))]
        string? typeName = null)
    {
        if (type is null)
            throw new ArgumentNullException(typeName);
        if (!type.Implements<Exception>())
            throw new ArgumentOutOfRangeException(typeName, type, "Type is not an Exception type");
    }
    
    public static void IsValueType([AllowNull, NotNull] Type? type,
        [CallerArgumentExpression(nameof(type))]
        string? typeName = null)
    {
        if (type is null)
            throw new ArgumentNullException(typeName);
        if (!type.IsValueType)
            throw new ArgumentOutOfRangeException(typeName, type, "Type is not a ValueType");
    }
    
    public static void IsClassType([AllowNull, NotNull] Type? type,
        [CallerArgumentExpression(nameof(type))]
        string? typeName = null)
    {
        if (type is null)
            throw new ArgumentNullException(typeName);
        if (!type.IsClass)
            throw new ArgumentOutOfRangeException(typeName, type, "Type is not a class type");
    }
    
    public static void IsInterfaceType([AllowNull, NotNull] Type? type,
        [CallerArgumentExpression(nameof(type))]
        string? typeName = null)
    {
        if (type is null)
            throw new ArgumentNullException(typeName);
        if (!type.IsInterface)
            throw new ArgumentOutOfRangeException(typeName, type, "Type is not an interface type");
    }
    
    public static void IsClassOrInterfaceType([AllowNull, NotNull] Type? type,
        [CallerArgumentExpression(nameof(type))]
        string? typeName = null)
    {
        if (type is null)
            throw new ArgumentNullException(typeName);
        if (!type.IsClass && !type.IsInterface)
            throw new ArgumentOutOfRangeException(typeName, type, "Type is not a class or interface type");
    }
    
    public static void IsInstance([AllowNull, NotNull] MemberInfo? member,
        [CallerArgumentExpression(nameof(member))]
        string? memberName = null)
    {
        if (member is null)
            throw new ArgumentNullException(memberName);
        if (member.IsStatic())
            throw new ArgumentOutOfRangeException(memberName, member, $"{member.NameOf()} is not an instance member");
    }
    
    public static void IsStatic([AllowNull, NotNull] MemberInfo? member,
        [CallerArgumentExpression(nameof(member))]
        string? memberName = null)
    {
        if (member is null)
            throw new ArgumentNullException(memberName);
        if (!member.IsStatic())
            throw new ArgumentOutOfRangeException(memberName, member, $"{member.NameOf()} is not a static member");
    }
}