namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class BooleanExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Not(this bool boolean)
        => !boolean;
}