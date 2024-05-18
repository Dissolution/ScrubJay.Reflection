using System.Linq.Expressions;
using ScrubJay.Reflection.Searching.Criteria;

namespace ScrubJay.Reflection.Searching;

public class TypeSearch<T> : TypeSearch
{
    public TypeSearch() : base(typeof(T)) { }
    
    public Result<TMember, Reflexception> TryFindMember<TMember>(Expression<Action<T>> memberExpression)
    {
        var member = Expressions.Expressions.FirstMember(memberExpression);
        if (member is TMember tMember)
            return tMember;
        return new Reflexception($"Could not find a member in {memberExpression}");
    }
    
    public Result<TMember, Reflexception> TryFindMember<TMember>(Expression<Func<T, object?>> memberExpression)
    {
        var member = Expressions.Expressions.FirstMember(memberExpression);
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

    public IEnumerable<MemberInfo> FindMembers(MemberCriteria criteria)
    {
        var members = _type.GetMembers(criteria.BindingFlags);
        foreach (MemberInfo member in members)
        {
            if (criteria.Matches(member))
                yield return member;
        }
    }

    public MemberInfo? FindMember(MemberCriteria criteria) => FindMembers(criteria).OneOrDefault();

    public IEnumerable<TMember> FindMembers<TMember>(Func<MemberCriteriaBuilder, ICriteria<TMember>> buildCriteria)
        where TMember : MemberInfo
    {
        var builder = new MemberCriteriaBuilder();
        ICriteria<TMember> memberCriteria = buildCriteria(builder);
        var members = _type
            .AllMembers()
            .OfType<TMember>();
        foreach (TMember member in members)
        {
            if (memberCriteria.Matches(member))
                yield return member;
        }
    }

    public TMember? FindMember<TMember>(Func<MemberCriteriaBuilder, ICriteria<TMember>> buildCriteria)
        where TMember : MemberInfo
        => FindMembers<TMember>(buildCriteria).OneOrDefault();

    public Result<TMember, MemberAccessException> TryFindMember<TMember>(
        Func<MemberCriteriaBuilder, ICriteria<TMember>> buildCriteria)
        where TMember : MemberInfo
    {
        var mcBuilder = new MemberCriteriaBuilder();
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