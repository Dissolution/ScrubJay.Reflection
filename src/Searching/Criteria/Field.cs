namespace ScrubJay.Reflection.Searching.Criteria;

public interface IFieldCriterion : IMemberCriterion<FieldInfo>
{
    ICriterion<Type> Type { get; set; }
    FieldModifiers Modifiers { get; set; }
}

public record class FieldCriterion : MemberCriterion<FieldInfo>, IFieldCriterion
{
    public ICriterion<Type> Type { get; set; } = Criterion<Type>.Pass;
    public FieldModifiers Modifiers { get; set; }

    public FieldCriterion() : base() { }
    public FieldCriterion(IMemberCriterion criterion) : base(criterion) { }
    public FieldCriterion(IFieldCriterion criterion) : base(criterion)
    {
        this.Type = criterion.Type;
        this.Modifiers = criterion.Modifiers;
    }
    

    public override bool Matches(FieldInfo? field)
    {
        if (!base.Matches(field))
            return false;

        if (!Type.Matches(field.FieldType))
            return false;
        
        if (!Modifiers.HasAnyFlags(field.Modifiers()))
            return false;

        return true;
    }
}

public interface IFieldCriterionBuilder<out TBuilder> : 
    IMemberBaseCriterionBuilder<TBuilder, IFieldCriterion, FieldInfo>
    where TBuilder : IFieldCriterionBuilder<TBuilder>
{
    TBuilder Type(ICriterion<Type> type);
    TBuilder Type(Type type, TypeMatch match = TypeMatch.Exact);
    TBuilder Type<TField>(TypeMatch match = TypeMatch.Exact);
    
    TBuilder Modifiers(FieldModifiers modifiers);
    
    TBuilder Like(FieldInfo field);
}

internal class FieldCriterionBuilder<TBuilder> :
    MemberCriterionBuilder<TBuilder, IFieldCriterion, FieldInfo>,
    IFieldCriterionBuilder<TBuilder>
    where TBuilder : IFieldCriterionBuilder<TBuilder>
{
    protected FieldCriterionBuilder(IFieldCriterion criterion) : base(criterion)
    {
    }

    public TBuilder Type(ICriterion<Type> type)
    {
        _criterion.Type = type;
        return _builder;
    }
    public TBuilder Type(Type type, TypeMatch match = TypeMatch.Exact)
        => Type(Criterion.Match(type, match));
    public TBuilder Type<TField>(TypeMatch match = TypeMatch.Exact)
        => Type(Criterion.Match(typeof(TField), match));
    public TBuilder Modifiers(FieldModifiers modifiers)
    {
        _criterion.Modifiers = modifiers;
        return _builder;
    }

    public TBuilder Like(FieldInfo field)
    {
        base.Like(field);
        _criterion.Type = Criterion.Match(field.FieldType);
        return _builder;
    }
}

public interface IFieldCriterionBuilderImpl : IFieldCriterionBuilder<IFieldCriterionBuilderImpl>;

internal class FieldCriterionBuilderImpl : 
    FieldCriterionBuilder<IFieldCriterionBuilderImpl>,
    IFieldCriterionBuilderImpl
{
    public FieldCriterionBuilderImpl() : base(new FieldCriterion()) { }
    public FieldCriterionBuilderImpl(IMemberCriterion criterion) : base(new FieldCriterion(criterion)) { }
    public FieldCriterionBuilderImpl(IFieldCriterion criterion) : base(new FieldCriterion(criterion)) { }
}