using System.Diagnostics;
using ScrubJay.Extensions;
using ScrubJay.Reflection.Extensions;

namespace ScrubJay.Reflection.Searching;

public static class Mirror
{
    private static IEnumerable<MemberInfo> MatchMembers(Type type, MemberCriteria criteria)
    {
        var members = type.GetMembers(criteria.BindingFlags);
        foreach (MemberInfo member in members)
        {
            if (!criteria.Visibility.HasFlags(member.Visibility()))
                continue;
            if (!criteria.MemberType.HasFlags(member.MemberType))
                continue;
            if (criteria.Name is not null)
            {
                if (criteria.NameCriteria == NameCriteria.Exact)
                {
                    if (!string.Equals(member.Name, criteria.Name, criteria.NameComparison))
                        continue;
                }
                else if (criteria.NameCriteria.HasFlags(NameCriteria.Contains))
                {
                    if (!member.Name.Contains(criteria.Name, criteria.NameComparison))
                        continue;
                }
                else
                {
                    if (criteria.NameCriteria.HasFlags(NameCriteria.StartsWith))
                    {
                        if (!member.Name.StartsWith(criteria.Name, criteria.NameComparison))
                            continue;
                    }
                    if (criteria.NameCriteria.HasFlags(NameCriteria.EndsWith))
                    {
                        if (!member.Name.EndsWith(criteria.Name, criteria.NameComparison))
                            continue;
                    }
                }
            }
            
            // passes
            yield return member;
        }
    }

    private static IEnumerable<FieldInfo> MatchFields(Type type, FieldCriteria criteria)
    {
        foreach (var member in MatchMembers(type, criteria))
        {
            Debug.Assert(member.MemberType == MemberTypes.Field);
            Debug.Assert(member is FieldInfo);
            FieldInfo field = (FieldInfo)member;
            if (criteria.FieldType is not null)
            {
                if (!field.FieldType.Passes(criteria.TypeCriteria, criteria.FieldType))
                    continue;
            }
            if (criteria.Modifiers != FieldModifiers.Any)
            {
                if (criteria.Modifiers.HasFlags(FieldModifiers.InitOnly) != field.IsInitOnly)
                    continue;
                if (criteria.Modifiers.HasFlags(FieldModifiers.Const) != field.IsLiteral)
                    continue;
                if (criteria.Modifiers.HasFlags(FieldModifiers.HasDefault) != field.Attributes.HasFlags(FieldAttributes.HasDefault))
                    continue;
            }

            yield return field;
        }
    }
    
    
    public static MemberInfo? FindMember(Type type, MemberCriteria criteria)
    {
        return MatchMembers(type, criteria).OneOrDefault();
    }

    public static MemberInfo? FindMember<T>(MemberCriteria criteria)
    {
        return MatchMembers(typeof(T), criteria).OneOrDefault();
    }
    
    public static IEnumerable<MemberInfo> FindMembers(Type type, MemberCriteria criteria)
    {
        return MatchMembers(type, criteria);
    }
    
    public static IEnumerable<MemberInfo> FindMembers<T>(MemberCriteria criteria)
    {
        return MatchMembers(typeof(T), criteria);
    }

    public static TMember? FindMember<TMember>(Type type, MemberCriteria criteria)
        where TMember : MemberInfo
    {
        return MatchMembers(type, criteria).OneOrDefault() as TMember;
    }
    
    public static TMember? FindMember<T, TMember>(MemberCriteria criteria)
        where TMember : MemberInfo
    {
        return MatchMembers(typeof(T), criteria).OneOrDefault() as TMember;
    }
    
    public static IEnumerable<TMember> FindMembers<TMember>(Type type, MemberCriteria criteria)
        where TMember : MemberInfo
    {
        return MatchMembers(type, criteria).OfType<TMember>();
    }
    
    public static IEnumerable<TMember> FindMembers<T, TMember>(MemberCriteria criteria)
        where TMember : MemberInfo
    {
        return MatchMembers(typeof(T), criteria).OfType<TMember>();
    }

    public static FieldInfo? FindField(Type type, FieldCriteria criteria)
    {
        return MatchFields(type, criteria).OneOrDefault();
    }
    
    public static FieldInfo? FindField<T>(FieldCriteria criteria)
    {
        return MatchFields(typeof(T), criteria).OneOrDefault();
    }
    
    public static IEnumerable<FieldInfo> FindFields(Type type, FieldCriteria criteria)
    {
        return MatchFields(type, criteria);
    }
    
    public static IEnumerable<FieldInfo> FindFields<T>(FieldCriteria criteria)
    {
        return MatchFields(typeof(T), criteria);
    }
}