namespace ScrubJay.Reflection.Extensions;

public static class StringExtensions
{
#region Compat
#if NET481 || NETSTANDARD2_0
        public static bool Contains(this string str, string value, StringComparison comparisonType)
        {
            return str.IndexOf(value, comparisonType) >= 0;
        }
#endif
#endregion
}