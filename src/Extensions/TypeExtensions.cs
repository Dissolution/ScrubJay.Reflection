namespace ScrubJay.Reflection.Extensions;

/// <summary>
/// Extensions on <see cref="Type"/>
/// </summary>
[PublicAPI]
public static class TypeExtensions
{
    public static bool IsNullOrVoid(this Type? type)
    {
        return type is null || type == typeof(void);
    }
}