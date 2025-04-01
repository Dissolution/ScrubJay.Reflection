using Acc = ScrubJay.Reflection.Access;
using Vis = ScrubJay.Reflection.Visibility;
using BF = System.Reflection.BindingFlags;

namespace ScrubJay.Reflection.Extensions;

/// <summary>
/// Extensions on <see cref="BindingFlags"/>
/// </summary>
[PublicAPI]
public static class BindingFlagsExtensions
{
    public static Acc Access(this BF bindingFlags)
    {
        Acc access = Acc.None;
        if (bindingFlags.HasFlags(BF.Instance))
            access.AddFlag(Acc.Instance);
        if (bindingFlags.HasFlags(BF.Static))
            access.AddFlag(Acc.Static);
        return access;
    }

    public static Vis Visibility(this BF bindingFlags)
    {
        Vis visibility = Vis.None;
        if (bindingFlags.HasFlags(BF.Public))
            visibility.AddFlag(Vis.Public);
        if (bindingFlags.HasFlags(BF.NonPublic))
            visibility.AddFlag(Vis.NonPublic);
        return visibility;
    }
}