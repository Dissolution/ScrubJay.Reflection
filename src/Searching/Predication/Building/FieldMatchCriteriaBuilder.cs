using ScrubJay.Reflection.Searching.Predication.Members;

namespace ScrubJay.Reflection.Searching.Predication.Building;

public sealed class FieldMatchCriteriaBuilder : FieldMatchCriteriaBuilder<FieldMatchCriteriaBuilder>
{
    public FieldMatchCriteriaBuilder() : base()
    {
    }

    public FieldMatchCriteriaBuilder(FieldMatchCriteria criteria) : base(criteria)
    {
    }

    public FieldMatchCriteriaBuilder(MemberMatchCriteria<MemberInfo> criteria) : base(criteria)
    {
    }
}

public class FieldMatchCriteriaBuilder<TBuilder> : MemberMatchCriteriaBuilder<TBuilder, FieldMatchCriteria, FieldInfo>
    where TBuilder : FieldMatchCriteriaBuilder<TBuilder>
{
    public FieldMatchCriteriaBuilder() : base()
    {
    }

    public FieldMatchCriteriaBuilder(FieldMatchCriteria criteria) : base(criteria)
    {
    }

    public FieldMatchCriteriaBuilder(MemberMatchCriteria<MemberInfo> criteria) : base(criteria)
    {
    }

    public TBuilder ValueType(ICriteria<Type> type)
    {
        _criteria.ValueType = type;
        return _builder;
    }

    public TBuilder ValueType(Type type, TypeMatchType match = TypeMatchType.Exact) => ValueType(new TypeEqualityCriteria(type, match));

    public TBuilder ValueType<TField>(TypeMatchType match = TypeMatchType.Exact) => ValueType(new TypeEqualityCriteria(typeof(TField), match));

    public TBuilder Modifiers(FieldModifiers modifiers)
    {
        _criteria.Modifiers = modifiers;
        return _builder;
    }

    public TBuilder InheritFrom(FieldInfo field)
    {
        base.InheritFrom(field);
        _criteria.ValueType = new TypeEqualityCriteria(field.FieldType);
        return _builder;
    }
}

