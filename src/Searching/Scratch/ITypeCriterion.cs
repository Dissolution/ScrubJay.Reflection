namespace ScrubJay.Reflection.Searching.Scratch;

public interface ITypeCriterion : IMemberCriterion<Type>, 
    IGenericTypesCriterion
{
    ICriterion<Type>? NestedType { get; set; }
}

public record class TypeCriterion : MemberCriterion<Type>, ITypeCriterion
{
    public ICriterion<Type>[]? GenericTypes { get; set; }
    public ICriterion<Type>? NestedType { get; set; }

    public TypeCriterion() : base() { }
    public TypeCriterion(IMemberCriterion criterion) : base(criterion) { }
    public TypeCriterion(ITypeCriterion criterion) : base(criterion)
    {
        this.GenericTypes = criterion.GenericTypes;
        this.NestedType = criterion.NestedType;
    }
    
    public override bool Matches([NotNullWhen(true)] Type? type)
    {
        if (!base.Matches(type))
            return false;
        
        if (GenericTypes is not null)
        {
            var methodGenericTypes = type.GetGenericArguments();
            if (methodGenericTypes.Length != GenericTypes.Length)
                return false;
            for (var i = 0; i < methodGenericTypes.Length; i++)
            {
                if (!GenericTypes[i].Matches(methodGenericTypes[i]))
                    return false;
            }
        }

        if (NestedType is not null && !NestedType.Matches(type.DeclaringType))
            return false;

        return true;
    }
}

public interface ITypeCriterionBuilder<out TBuilder> : 
    IMemberCriterionBuilder<TBuilder, ITypeCriterion, Type>
    where TBuilder : ITypeCriterionBuilder<TBuilder>
{
    TBuilder NestedType(ICriterion<Type> nestedType);
    TBuilder NestedType(Type type, TypeMatch typeMatch = TypeMatch.Exact);
    TBuilder NestedType<T>(TypeMatch typeMatch = TypeMatch.Exact);

    TBuilder GenericTypes(params ICriterion<Type>[] genericTypes);
    TBuilder GenericTypes(params Type[] genericTypes);
}

internal class TypeCriterionBuilder<TBuilder> : MemberCriterionBuilder<TBuilder, ITypeCriterion, Type>, 
    ITypeCriterionBuilder<TBuilder>
    where TBuilder : ITypeCriterionBuilder<TBuilder>
{
    protected TypeCriterionBuilder(ITypeCriterion criterion) : base(criterion)
    {
    }

    public TBuilder NestedType(ICriterion<Type> nestedType)
    {
        _criterion.NestedType = nestedType;
        return _builder;
    }
    public TBuilder NestedType(Type type, TypeMatch typeMatch = TypeMatch.Exact)
    {
        _criterion.NestedType = new TypeMatchCriterion { Type = type, TypeMatch = typeMatch };
        return _builder;
    }
    public TBuilder NestedType<T>(TypeMatch typeMatch = TypeMatch.Exact)
        => NestedType(typeof(T), typeMatch);
    
    public TBuilder GenericTypes(params ICriterion<Type>[] genericTypes)
    {
        _criterion.GenericTypes = genericTypes;
        return _builder;
    }
    public TBuilder GenericTypes(params Type[] genericTypes)
    {
        _criterion.GenericTypes = genericTypes.ConvertAll<Type, ICriterion<Type>>(static type => new TypeMatchCriterion(type));
        return _builder;
    }
}

public interface ITypeCriterionBuilderImpl : ITypeCriterionBuilder<ITypeCriterionBuilderImpl>;

internal class TypeCriterionBuilderImpl : TypeCriterionBuilder<ITypeCriterionBuilderImpl>,
    ITypeCriterionBuilderImpl
{
    public TypeCriterionBuilderImpl() : base(new TypeCriterion()) { }
    public TypeCriterionBuilderImpl(IMemberCriterion criterion) : base(new TypeCriterion(criterion)) { }
    public TypeCriterionBuilderImpl(ITypeCriterion criterion) : base(criterion) { }
}