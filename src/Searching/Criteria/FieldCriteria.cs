using ScrubJay.Validation;

namespace ScrubJay.Reflection.Searching.Criteria;

[Flags]
public enum FieldModifiers
{
    None = 0,
    InitOnly = 1 << 0,
    Const = 1 << 1,
    HasDefault = 1 << 2,
    
    Any = InitOnly | Const | HasDefault,
}

public record class FieldCriteria : MemberCriteria, ICriteria<FieldInfo>
{
    public static FieldCriteria Create(Type fieldType, FieldModifiers modifiers = FieldModifiers.Any)
    {
        ThrowIf.Null(fieldType);
        return new()
        {
            Type = fieldType,
            Modifiers = modifiers,
        };
    }
    public static new FieldCriteria Create(MemberCriteria criteria)
    {
        return new()
        {
            Access = criteria.Access,
            MemberType = criteria.MemberType,
            Name = criteria.Name,
            Visibility = criteria.Visibility,
        };
    }
    
    public TypeCriteria? Type { get; set; } = null;
    public FieldModifiers Modifiers { get; set; } = FieldModifiers.Any;
    
    public override MemberTypes MemberType => MemberTypes.Field;

    public bool Matches(FieldInfo? field)
    {
        if (!base.Matches(field))
            return false;

        if (Type is not null && !Type.Matches(field.FieldType))
            return false;
        if (Modifiers != FieldModifiers.Any)
        {
            if (Modifiers.HasFlags(FieldModifiers.InitOnly) != field.IsInitOnly)
                return false;
            if (Modifiers.HasFlags(FieldModifiers.Const) != field.IsLiteral)
                return false;
            if (Modifiers.HasFlags(FieldModifiers.HasDefault) != field.Attributes.HasFlags(FieldAttributes.HasDefault))
                return false;
        }
        return true;
    }
}

public abstract class FieldCriteriaBuilder<TBuilder, TCriteria> : MemberCriteriaBuilder<TBuilder, TCriteria>
    where TBuilder : MemberCriteriaBuilder<TBuilder, TCriteria>
    where TCriteria : FieldCriteria, new()
{
    protected FieldCriteriaBuilder() { }
    protected FieldCriteriaBuilder(TCriteria criteria) : base(criteria) { }

    public TBuilder FieldType(TypeCriteria criteria)
    {
        _criteria.Type = criteria;
        return _builder;
    }

    public TBuilder FieldType<TField>()
    {
        _criteria.Type = typeof(TField);
        return _builder;
    }

    public TBuilder Modifiers(FieldModifiers modifiers)
    {
        _criteria.Modifiers = modifiers;
        return _builder;
    }
}

public sealed class FieldCriteriaBuilder : FieldCriteriaBuilder<FieldCriteriaBuilder, FieldCriteria>, ICriteria<FieldInfo>
{
    internal FieldCriteriaBuilder()
    {
    }
    internal FieldCriteriaBuilder(FieldCriteria criteria) : base(criteria)
    {
    }

    public bool Matches(FieldInfo? field) => _criteria.Matches(field);
}