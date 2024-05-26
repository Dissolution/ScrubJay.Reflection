using System.Linq.Expressions;
using ScrubJay.Reflection.Searching.Criteria;
using ScrubJay.Validation;
using ParameterInfo = System.Reflection.ParameterInfo;

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

    public static ICriterion<ParameterInfo[]> GetParameterCriteria(params object?[]? arguments)
    {
        if (arguments is null)
            return Criterion<ParameterInfo[]>.Pass;

        var criteria = arguments.ConvertAll<object?, ICriterion<ParameterInfo>>(obj =>
        {
            ICriterion<ParameterInfo> criterion;
            if (obj is null)
            {
                criterion = Criterion<ParameterInfo>.Pass;
            }
            else
            {
                Type argType = obj.GetType();
                criterion = TypeMatchCriterion.Create<ParameterInfo>(
                    p => p?.ParameterType,
                    argType, TypeMatch.ImplementedBy);
            }
            return criterion;
        });

        return Criterion.Combine(criteria);
    }
}