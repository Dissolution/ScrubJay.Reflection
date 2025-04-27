using ScrubJay.Reflection.IL.Decompilation;
using ScrubJay.Reflection.IL.Instructions;

namespace ScrubJay.Reflection.Extensions;

public static class PropertyInfoExtensions
{
    internal static bool IsInitOnly(MethodInfo? setMethod)
    {
        // Get the modifiers applied to the return parameter
        if (setMethod?.ReturnParameter is null)
            return false;

        var customModifiers = setMethod.ReturnParameter.GetRequiredCustomModifiers();

        // Init-only properties are marked with the IsExternalInit type
        return Sequence.Contains(customModifiers, typeof(IsExternalInit));
    }

    /// <summary>
    /// Determines if this property is marked as init-only.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>True if the property is init-only, false otherwise.</returns>
    public static bool IsInitOnly(this PropertyInfo property)
    {
        if (!property.CanWrite)
        {
            Debug.Assert(property.SetMethod is null);
            return false;
        }

        return IsInitOnly(property.SetMethod);
    }

    private static string GetBackingFieldName(PropertyInfo property) => $"<{property.Name}>k__BackingField";

    public static FieldInfo? GetBackingField(this PropertyInfo? property)
    {
        if (property is null)
            return null;

        string fieldName = GetBackingFieldName(property);

        BF flags = BF.DeclaredOnly | BF.NonPublic;

        if (property.IsStatic())
        {
            flags |= BF.Static;
        }
        else
        {
            flags |= BF.Instance;
        }

        var ownerType = property.OwnerType();

        var backingField = ownerType.GetField(fieldName, flags);
        if (backingField is not null)
            return backingField;

        backingField = FindLastFieldReference(property.GetMethod, ownerType, property.PropertyType, property.Name);
        if (backingField is not null)
            return backingField;

        backingField = FindLastFieldReference(property.SetMethod, ownerType, property.PropertyType, property.Name);
        if (backingField is not null)
            return backingField;

        return null;
    }

    private static FieldInfo? FindLastFieldReference(MethodBase? method, Type? declaringType, Type fieldType, string name)
    {
        if (method is null)
            return null;

        return DecompiledILMethod.Decompile(method)
            .Instructions
            .Reverse()
            .OfType<OpCodeFieldInstruction>()
            .Where(static instr => instr.Field is not null)
            .SelectWhere(instr =>
            {
                if (instr.Field!.DeclaringType == declaringType &&
                    instr.Field.FieldType == fieldType)
                {
                    return Some(instr.Field);
                }
                return None();
            })
            .OrderBy(fld => TextHelper.LevenshteinDistance(fld.Name.AsSpan(), name.AsSpan()))
            .TryGetFirst()
            .OkOrDefault();
    }
}