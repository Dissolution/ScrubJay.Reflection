using ScrubJay.Reflection.Searching.Predication.Members;

namespace ScrubJay.Reflection.Searching.Predication.Building;

public sealed class TypeMatchCriteriaBuilder : TypeMatchCriteriaBuilder<TypeMatchCriteriaBuilder>
{
    public TypeMatchCriteriaBuilder() : base()
    {
    }

    public TypeMatchCriteriaBuilder(TypeMatchCriteria criteria) : base(criteria)
    {
    }

    public TypeMatchCriteriaBuilder(MemberMatchCriteria<MemberInfo> criteria) : base(criteria)
    {
    }
}

public class TypeMatchCriteriaBuilder<TBuilder> : MemberMatchCriteriaBuilder<TBuilder, TypeMatchCriteria, Type>
    where TBuilder : TypeMatchCriteriaBuilder<TBuilder>
{
    public TypeMatchCriteriaBuilder() : base()
    {
    }

    public TypeMatchCriteriaBuilder(TypeMatchCriteria criteria) : base(criteria)
    {
    }

    public TypeMatchCriteriaBuilder(MemberMatchCriteria<MemberInfo> criteria) : base(criteria)
    {
    }

    public TBuilder GenericTypes(ICriteria<Type[]> types)
    {
        _criteria.GenericTypes = types;
        return _builder;
    }
    
    public TBuilder GenericTypes(params ICriteria<Type>[] types)
    {
        _criteria.GenericTypes = Criteria<Type>.ArrayOfCriteriaToCriteriaOfArray(types);
        return _builder;
    }

    public TBuilder GenericTypes(params Type[] genericTypes)
    {
        return GenericTypes(genericTypes.ConvertAll<Type, ICriteria<Type>>(static type => Criteria.Equals(type)));
    }

    public TBuilder InheritFrom(Type type)
    {
        base.InheritFrom(type);
        _criteria.GenericTypes = Criteria<Type>.ArrayOfCriteriaToCriteriaOfArray(
            type.GetGenericArguments()
            .ConvertAll(static type => Criteria.Equals(type)));
        return _builder;
    }
}