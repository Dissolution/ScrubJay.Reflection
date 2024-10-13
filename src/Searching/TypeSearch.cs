using System.Linq.Expressions;
using ScrubJay.Reflection.Expressions;
using ScrubJay.Reflection.Searching.Predication;
using ScrubJay.Reflection.Searching.Predication.Building;

namespace ScrubJay.Reflection.Searching;

public class TypeSearch<T> : TypeSearch
{
    public TypeSearch() : base(typeof(T)) { }
    
    public Result<TMember, Reflexception> TryFindMember<TMember>(Expression<Action<T>> memberExpression)
    {
        var member = ExpressionTree.EnumerateMembers(memberExpression, Scope.Self | Scope.Children).FirstOrDefault();
        if (member is TMember tMember)
            return tMember;
        return new Reflexception($"Could not find a member in {memberExpression}");
    }
    
    public Result<TMember, Reflexception> TryFindMember<TMember>(Expression<Func<T, object?>> memberExpression)
    {
        var member = ExpressionTree.EnumerateMembers(memberExpression, Scope.Self | Scope.Children).FirstOrDefault();
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

    public IEnumerable<MemberInfo> FindMembers(ICriteria<MemberInfo> memberCriteria)
    {
        var members = _type.AllMembers();
        foreach (MemberInfo member in members)
        {
            if (memberCriteria.Matches(member))
                yield return member;
        }
    }

    public MemberInfo? FindMember(ICriteria<MemberInfo> memberCriteria) => FindMembers(memberCriteria).OneOrDefault();
    
    public IEnumerable<TMember> FindMembers<TMember>(Func<MemberMatchCriteriaBuilder, ICriteria<TMember>> buildCriteria)
        where TMember : MemberInfo
    {
        var builder = new MemberMatchCriteriaBuilder();
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

    public TMember? FindMember<TMember>(Func<MemberMatchCriteriaBuilder, ICriteria<TMember>> buildCriteria)
        where TMember : MemberInfo
        => FindMembers<TMember>(buildCriteria).OneOrDefault();

    public Result<TMember, MemberAccessException> TryFindMember<TMember>(
        Func<MemberMatchCriteriaBuilder, ICriteria<TMember>> buildCriteria)
        where TMember : MemberInfo
    {
        var mcBuilder = new MemberMatchCriteriaBuilder();
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
    
    public Result<TMember, Reflexception> TryFindMember<TMember>(Expression memberExpression)
    {
        var member = ExpressionTree.EnumerateMembers(memberExpression, Scope.Self | Scope.Children).FirstOrDefault();
        if (member is TMember tMember)
            return tMember;
        return new Reflexception($"Could not find a member in {memberExpression}");
    }
}