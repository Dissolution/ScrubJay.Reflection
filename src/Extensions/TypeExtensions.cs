namespace ScrubJay.Reflection.Extensions;

public static class TypeExtensions
{
    public static ReferenceType IsReference(this Type type)
    {
        if (type.IsByRef)
        {
            return Reflection.ReferenceType.Ref;
        }
        return Reflection.ReferenceType.Default;
    }
    
    public static ReferenceType ReferenceType(this Type type, out Type nonRefType)
    {
        if (type.IsByRef)
        {
            nonRefType = type.GetElementType().ThrowIfNull();
            return Reflection.ReferenceType.Ref;
        }
        nonRefType = type;
        return Reflection.ReferenceType.Default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MemberInfo[] AllMembers(this Type type)
    {
        return type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase);
    }
}