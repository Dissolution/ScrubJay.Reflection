using System.Linq.Expressions;
using ScrubJay.Reflection.Searching.Predication;
using ScrubJay.Reflection.Searching.Predication.Members;
using ScrubJay.Validation;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace ScrubJay.Reflection.Searching;

public static class Mirror
{
    public static TypeSearch<T> Search<T>() => new TypeSearch<T>();
    
    public static TypeSearch Search(Type type)
    {
        Validate.ThrowIfNull(type);
        return new(type);
    }
    
    public static Result<TMember, Reflexception> TryFindMember<TMember>(Expression memberExpression)
    {
        var member = Expressions.ExpressionHelper.FindMembers(memberExpression).FirstOrDefault();
        if (member is TMember tMember)
            return tMember;
        return new Reflexception($"Could not find a member in {memberExpression}");
    }

    public static ICriteria<ParameterInfo[]> GetParameterCriteria(params object?[]? arguments)
    {
        if (arguments is null)
            return Criteria<ParameterInfo[]>.Pass;
    
        var criteria = arguments.ConvertAll<object?, ICriteria<ParameterInfo>>(obj =>
        {
            ICriteria<ParameterInfo> criteria;
            if (obj is null)
            {
                criteria = Criteria<ParameterInfo>.Pass;
            }
            else
            {
                criteria = new ParameterMatchCriteria()
                {
                    ValueType = Criteria.Equals(obj.GetType(), TypeMatchType.ImplementedBy),
                };
            }
            return criteria;
        });
    
        return Criteria<ParameterInfo>.ArrayOfCriteriaToCriteriaOfArray(criteria);
    }
}