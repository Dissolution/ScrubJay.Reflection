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
}