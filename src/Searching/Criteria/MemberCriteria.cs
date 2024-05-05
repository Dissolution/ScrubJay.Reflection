using ScrubJay.Reflection.Extensions;

namespace ScrubJay.Reflection.Searching.Criteria;

public record class MemberCriteria : Criteria, ICriteria<MemberInfo>
{
    public static MemberCriteria Create(MemberCriteria criteria)
    {
        return new()
        {
            Name = criteria.Name,
            Visibility = criteria.Visibility,
            Access = criteria.Access,
            MemberType = criteria.MemberType,
        };
    }
    
    public NameCriteria? Name { get; set; } = null;
    public Visibility Visibility { get; set; } = Visibility.Any;
    public Access Access { get; set; } = Access.Any;
    public virtual MemberTypes MemberType { get; set; } = MemberTypes.All;

    internal BindingFlags BindingFlags
    {
        get
        {
            BindingFlags flags = default;
            if (Visibility.HasFlag(Visibility.Private))
                flags |= BindingFlags.NonPublic;
            if (Visibility.HasFlag(Visibility.Protected))
                flags |= BindingFlags.NonPublic;
            if (Visibility.HasFlag(Visibility.Internal))
                flags |= BindingFlags.NonPublic;
            if (Visibility.HasFlag(Visibility.Public))
                flags |= BindingFlags.Public;
            if (Access.HasFlag(Access.Static))
                flags |= BindingFlags.Static;
            if (Access.HasFlag(Access.Instance))
                flags |= BindingFlags.Instance;
            return flags;
        }
    }

    public bool Matches([NotNullWhen(true)] MemberInfo? member)
    {
        if (member is null)
            return false;
        if (!MemberType.HasAnyFlags(member.MemberType))
            return false;
        if (!Visibility.HasAnyFlags(member.Visibility()))
            return false;
        if (!Access.HasAnyFlags(member.Access()))
            return false;
        if (Name is not null && !Name.Matches(member.Name))
            return false;
        return true;
    }
}



public abstract class MemberCriteriaBuilder<TBuilder, TCriteria> : CriteriaBuilder<TBuilder, TCriteria>
    where TBuilder : MemberCriteriaBuilder<TBuilder, TCriteria>
    where TCriteria : MemberCriteria, new()
{
    public FieldCriteriaBuilder Field => new(FieldCriteria.Create(_criteria));
    public PropertyCriteriaBuilder Property => new(PropertyCriteria.Create(_criteria));
    public EventCriteriaBuilder Event => new(EventCriteria.Create(_criteria));
    public MethodBaseCriteriaBuilder MethodBase => new(MethodBaseCriteria.Create(_criteria));
    public MethodCriteriaBuilder Method => new(MethodCriteria.Create(_criteria));
    public ConstructorCriteriaBuilder Constructor => new(ConstructorCriteria.Create(_criteria));

    protected MemberCriteriaBuilder() { }
    protected MemberCriteriaBuilder(TCriteria criteria) : base(criteria) { }

    public TBuilder Name(NameCriteria nameCriteria)
    {
        _criteria.Name = nameCriteria;
        return _builder;
    }

    public TBuilder Visibility(Visibility visibility)
    {
        _criteria.Visibility = visibility;
        return _builder;
    }

    public TBuilder Access(Access access)
    {
        _criteria.Access = access;
        return _builder;
    }

    public TBuilder Static
    {
        get
        {
            _criteria.Access = Reflection.Access.Static;
            return _builder;
        }
    }

    public TBuilder Instance
    {
        get
        {
            _criteria.Access = Reflection.Access.Instance;
            return _builder;
        }
    }

    public MemberCriteriaBuilder MemberType(MemberTypes memberType)
    {
        _criteria.MemberType = memberType;
        return new(_criteria);
    }
}

public sealed class MemberCriteriaBuilder : MemberCriteriaBuilder<MemberCriteriaBuilder, MemberCriteria>, ICriteria<MemberInfo>
{
    public MemberCriteriaBuilder()
    {
    }
    
    public MemberCriteriaBuilder(MemberCriteria criteria) : base(criteria)
    {
    }

    public bool Matches(MemberInfo? member) => _criteria.Matches(member);

    public override string ToString() => _criteria.ToString();
}



