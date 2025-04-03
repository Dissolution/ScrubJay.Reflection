namespace ScrubJay.Reflection.Extensions;

/// <summary>
/// Extensions on <see cref="BindingFlags"/>
/// </summary>
[PublicAPI]
public static class BindingFlagsExtensions
{
    public static Viz AsVisibility(this BF bindingFlags)
    {
        Viz visibility = default;
        if (bindingFlags.HasFlags(BF.Public))
            visibility.AddFlag(Viz.Public);
        if (bindingFlags.HasFlags(BF.NonPublic))
            visibility.AddFlag(Viz.NonPublic);
        if (bindingFlags.HasFlags(BF.Static))
            visibility.AddFlag(Viz.Static);
        if (bindingFlags.HasFlags(BF.Instance))
            visibility.AddFlag(Viz.Instance);
        return visibility;
    }
}