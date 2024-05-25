namespace ScrubJay.Reflection.Extensions;

[Flags]
public enum FieldModifiers
{
    None = 0,
    InitOnly = 1 << 0,
    Const = 1 << 1,
    HasDefault = 1 << 2,
    
    Any = InitOnly | Const | HasDefault,
}

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