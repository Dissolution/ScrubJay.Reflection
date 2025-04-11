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
}