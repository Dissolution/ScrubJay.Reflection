using System.Linq.Expressions;
using ScrubJay.Reflection.Searching.Criteria;

namespace ScrubJay.Reflection.Searching;

public class TypeSearch<T> : TypeSearch
{
    public TypeSearch() : base(typeof(T)) { }
    
    public Result<TMember, Reflexception> TryFindMember<TMember>(Expression<Action<T>> memberExpression)
    {
        var member = Expressions.Expressions.FindMembers(memberExpression).FirstOrDefault();
        if (member is TMember tMember)
            return tMember;
        return new Reflexception($"Could not find a member in {memberExpression}");
    }
    
    public Result<TMember, Reflexception> TryFindMember<TMember>(Expression<Func<T, object?>> memberExpression)
    {
        var member = Expressions.Expressions.FindMembers(memberExpression).FirstOrDefault();
        if (member is TMember tMember)
            return tMember;
        return new Reflexception($"Could not find a member in {memberExpression}");
    }
}

public class TypeSearch
{
    protected readonly Type _type;

    public TypeSearch(Type type)
    {
        _type = type;
    }

    public IEnumerable<MemberInfo> FindMembers(IMemberCriterion<MemberInfo> criteria)
    {
        var members = _type.GetMembers(criteria.BindingFlags());
        foreach (MemberInfo member in members)
        {
            if (criteria.Matches(member))
                yield return member;
        }
    }

    public MemberInfo? FindMember(IMemberCriterion<MemberInfo> criteria) => FindMembers(criteria).OneOrDefault();
    
    public IEnumerable<TMember> FindMembers<TMember>(Func<IMemberCriterionBuilderImpl, ICriterion<TMember>> buildCriteria)
        where TMember : MemberInfo
    {
        var builder = new MemberCriterionBuilderImpl();
        var memberCriteria = buildCriteria(builder);
        var members = _type
            .AllMembers()
            .OfType<TMember>();
        foreach (TMember member in members)
        {
            if (memberCriteria.Matches(member))
                yield return member;
        }
    }

    public TMember? FindMember<TMember>(Func<IMemberCriterionBuilderImpl, ICriterion<TMember>> buildCriteria)
        where TMember : MemberInfo
        => FindMembers<TMember>(buildCriteria).OneOrDefault();

    public Result<TMember, MemberAccessException> TryFindMember<TMember>(
        Func<IMemberCriterionBuilderImpl, ICriterion<TMember>> buildCriteria)
        where TMember : MemberInfo
    {
        var mcBuilder = new MemberCriterionBuilderImpl();
        var criteria = buildCriteria(mcBuilder);
        var member = _type
            .AllMembers()
            .OfType<TMember>()
            .Where(m => criteria.Matches(m))
            .OneOrDefault();
        if (member is not null)
            return member;
        return new MemberAccessException($"""
            Criteria:
            {criteria}
            Matched non-1 members
            """);
    }
}