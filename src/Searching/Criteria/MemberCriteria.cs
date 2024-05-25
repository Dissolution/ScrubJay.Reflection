using Vis = ScrubJay.Reflection.Visibility;
using Acc = ScrubJay.Reflection.Access;

namespace ScrubJay.Reflection.Searching.Criteria;

public record class MemberCriteria : Criteria, ICriteria<MemberInfo>
{
//    public static MemberCriteria Create(MemberCriteria criteria)
//    {
//        return new()
//        {
//            Name = criteria.Name,
//            Visibility = criteria.Visibility,
//            Access = criteria.Access,
//            MemberType = criteria.MemberType,
//        };
//    }
    
    public TextCriteria? Name { get; set; } = null;
    public Vis Visibility { get; set; } = Vis.Any;
    public Acc Access { get; set; } = Acc.Any;
    public virtual MemberTypes MemberType { get; set; } = MemberTypes.All;

    internal BindingFlags BindingFlags
    {
        get
        {
            BindingFlags flags = default;
            if (Visibility.HasFlags(Vis.Private))
                flags |= BindingFlags.NonPublic;
            if (Visibility.HasFlags(Vis.Protected))
                flags |= BindingFlags.NonPublic;
            if (Visibility.HasFlags(Vis.Internal))
                flags |= BindingFlags.NonPublic;
            if (Visibility.HasFlags(Vis.Public))
                flags |= BindingFlags.Public;
            if (Access.HasFlags(Acc.Static))
                flags |= BindingFlags.Static;
            if (Access.HasFlags(Acc.Instance))
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
    public TypeCriteriaBuilder Type => throw new NotImplementedException();//new TypeCriteria((TypeCriteria)_criteria));

    protected MemberCriteriaBuilder() { }
    protected MemberCriteriaBuilder(TCriteria criteria) : base(criteria) { }

    public TBuilder Name(TextCriteria textCriteria)
    {
        _criteria.Name = textCriteria;
        return _builder;
    }

    public TBuilder Visibility(Vis visibility)
    {
        _criteria.Visibility = visibility;
        return _builder;
    }
    
    public TBuilder Public
    {
        get
        {
            _criteria.Visibility = Vis.Public;
            return _builder;
        }
    }
    
    public TBuilder NonPublic
    {
        get
        {
            _criteria.Visibility = Vis.NonPublic;
            return _builder;
        }
    }
    
    public TBuilder Internal
    {
        get
        {
            _criteria.Visibility = Vis.Internal;
            return _builder;
        }
    }

    public TBuilder Protected
    {
        get
        {
            _criteria.Visibility = Vis.Protected;
            return _builder;
        }
    }

    public TBuilder Private
    {
        get
        {
            _criteria.Visibility = Vis.Private;
            return _builder;
        }
    }


    public TBuilder Access(Acc access)
    {
        _criteria.Access = access;
        return _builder;
    }

    public TBuilder Static
    {
        get
        {
            _criteria.Access = Acc.Static;
            return _builder;
        }
    }

    public TBuilder Instance
    {
        get
        {
            _criteria.Access = Acc.Instance;
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



