namespace ScrubJay.Reflection.Extensions;

/// <summary>
/// Extensions on <see cref="DynamicMethod"/>
/// </summary>
[PublicAPI]
public static class DynamicMethodExtensions
{
#if NETFRAMEWORK || NETSTANDARD
    public static D CreateDelegate<D>(this DynamicMethod dynamicMethod)
        where D : Delegate
    {
        return (D)dynamicMethod.CreateDelegate(typeof(D));
    }
#endif
}