using System.Reflection.Emit;

namespace ScrubJay.Reflection.Extensions;

/// <summary>
/// Extensions on <see cref="DynamicMethod"/>
/// </summary>
[PublicAPI]
public static class DynamicMethodExtensions
{
#if NETFRAMEWORK || NETSTANDARD
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDelegate CreateDelegate<TDelegate>(this DynamicMethod dynamicMethod)
        where TDelegate : Delegate
        => (TDelegate)dynamicMethod.CreateDelegate(typeof(TDelegate));
#endif
}