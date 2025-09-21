namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class HashSetExtensions
{
    extension<T>(HashSet<T> set)
    {
        public void AddMany(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                set.Add(item);
            }
        }
    }
}