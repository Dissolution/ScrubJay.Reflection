namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class AttributeExtensions
{
    public static bool HasAttribute(this MemberInfo? member, Type? attributeType, bool inherit = false)
    {
        if (member is null || attributeType is null)
            return false;
        
        try
        {
            return Attribute.IsDefined(member, attributeType, inherit);
        }
        catch
        {
            return false;
        }
    }
    
    public static bool HasAttribute<A>(this MemberInfo? member, bool inherit = false)
        where A : Attribute
        => HasAttribute(member, typeof(A), inherit);
    
    public static bool HasAttribute(this ParameterInfo? parameter, Type? attributeType, bool inherit = false)
    {
        if (parameter is null || attributeType is null)
            return false;
        
        try
        {
            return Attribute.IsDefined(parameter, attributeType, inherit);
        }
        catch
        {
            return false;
        }
    }
    
    public static bool HasAttribute<A>(this ParameterInfo? parameter, bool inherit = false)
        where A : Attribute
        => HasAttribute(parameter, typeof(A), inherit);
}