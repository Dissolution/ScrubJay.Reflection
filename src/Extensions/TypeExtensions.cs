using ScrubJay.Validation;
using RK = ScrubJay.Reflection.RefKind;

namespace ScrubJay.Reflection.Extensions;

public static class TypeExtensions
{
    public static RK RefKind(this Type type)
    {
        if (type.IsByRef)
        {
            return RK.Ref;
        }
        return RK.None;
    }
    
    public static RK RefKind(this Type type, out Type nonRefType)
    {
        if (type.IsByRef)
        {
            nonRefType = type.GetElementType().ThrowIfNull();
            return RK.Ref;
        }
        nonRefType = type;
        return RK.None;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MemberInfo[] AllMembers(this Type type)
    {
        return type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase);
    }
}