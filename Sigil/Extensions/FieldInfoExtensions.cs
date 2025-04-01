using System.Runtime.CompilerServices;

namespace ScrubJay.Sigil.Extensions;

/// <summary>
/// Extensions on <see cref="FieldInfo"/>
/// </summary>
[PublicAPI]
public static class FieldInfoExtensions
{
    public static bool IsVolatile(this FieldInfo field)
    {
        // field builder doesn't implement GetRequiredCustomModifiers
        if (field is FieldBuilder)
            return false;

        return Array.IndexOf<Type>(field.GetRequiredCustomModifiers(), typeof(IsVolatile)) >= 0;
    }
}
