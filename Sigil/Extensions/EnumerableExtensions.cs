namespace ScrubJay.Sigil.Extensions;

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> perItem)
    {
        foreach (var item in enumerable)
        {
            perItem(item);
        }
    }

    public static IEnumerable<T> Reversed<T>(this IEnumerable<T> enumerable)
    {
        return Enumerable.Reverse<T>(enumerable);
    }
}
