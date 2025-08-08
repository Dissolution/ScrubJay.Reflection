namespace ScrubJay.Reflection.Extensions;

/// <summary>
/// Extensions on <see cref="char"/>
/// </summary>
[PublicAPI]
public static class CharExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiHexDigitLower(this char ch)
    {
#if NET7_0_OR_GREATER
        return char.IsAsciiHexDigitLower(ch);
#else
        return (uint)(ch - '0') <= 9 || (uint)(ch - 'a') <= ('f' - 'a');
#endif
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiHexDigitUpper(this char ch)
    {
#if NET7_0_OR_GREATER
        return char.IsAsciiHexDigitUpper(ch);
#else
        return (uint)(ch - '0') <= 9 || (uint)(ch - 'A') <= ('F' - 'A');
#endif
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiHexDigit(this char ch)
    {
#if NET7_0_OR_GREATER
        return char.IsAsciiHexDigit(ch);
#else
        return (uint)(ch - '0') <= 9 || (uint)(ch - 'A') <= ('F' - 'A') || (uint)(ch - 'a') <= ('f' - 'a');
#endif
    }
}