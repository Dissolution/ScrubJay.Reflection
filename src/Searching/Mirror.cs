using System.Linq.Expressions;
using ScrubJay.Validation;

namespace ScrubJay.Reflection.Searching;

public static class Mirror
{
    public static TypeSearch<T> Search<T>() => new TypeSearch<T>();
    
    public static TypeSearch Search(Type type)
    {
        ThrowIf.Null(type);
        return new(type);
    }
    
    public static Result<TMember, Reflexception> TryFindMember<TMember>(Expression memberExpression)
    {
        var member = Expressions.Expressions.FindMembers(memberExpression).FirstOrDefault();
        if (member is TMember tMember)
            return tMember;
        return new Reflexception($"Could not find a member in {memberExpression}");
    }
}