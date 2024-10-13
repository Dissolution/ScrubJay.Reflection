namespace ScrubJay.Reflection.Extensions;

/// <summary>
/// Extensions on <see cref="FieldInfo"/>
/// </summary>
[PublicAPI]
public static class FieldInfoExtensions
{
    public static FieldModifiers Modifiers(this FieldInfo? field)
    {
        FieldModifiers modifiers = default;
        if (field is null) return modifiers;
        if (field.IsInitOnly)
            modifiers.AddFlag(FieldModifiers.InitOnly);
        if (field.IsLiteral)
            modifiers.AddFlag(FieldModifiers.Const);
        if (field.Attributes.HasFlags(FieldAttributes.HasDefault))
            modifiers.AddFlag(FieldModifiers.HasDefault);
        return modifiers;
    }
}