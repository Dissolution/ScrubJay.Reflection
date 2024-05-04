using ScrubJay.Extensions;
using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection.Extensions;

public static class TypeExtensions
{
    public static bool Passes([AllowNull, NotNullWhen(true)] this Type? type, TypeCriteria criteria, Type toCheck)
    {
        if (type is null) 
            return false;
        
        if (type == toCheck)
            return criteria.HasFlags(TypeCriteria.Exact);
       
        if (criteria.HasFlags(TypeCriteria.Implements) && type.Implements(toCheck))
            return true;

        if (criteria.HasFlags(TypeCriteria.ImplementedBy) && toCheck.Implements(type))
            return true;

        return false;
    }
}
