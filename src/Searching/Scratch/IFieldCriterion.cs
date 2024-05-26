namespace ScrubJay.Reflection.Searching.Scratch;

public interface IFieldCriterion : IMemberCriterion<FieldInfo>
{
    ICriterion<Type>? Type { get; set; }
    FieldModifiers Modifiers { get; set; }
}

public record class FieldCriterion : MemberCriterion<FieldInfo>, IFieldCriterion
{
    public ICriterion<Type>? Type { get; set; }
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

        if (Type is not null && !Type.Matches(field.FieldType))
            return false;
        if (!Modifiers.HasAnyFlags(field.Modifiers()))
            return false;

        return true;
    }
}

public interface IFieldCriterionBuilder<out TBuilder> : 
    IMemberCriterionBuilder<TBuilder, IFieldCriterion, FieldInfo>
    where TBuilder : IFieldCriterionBuilder<TBuilder>
{
    TBuilder Type(ICriterion<Type> criterion);
    TBuilder Type(Type type, TypeMatch typeMatch = TypeMatch.Exact);
    TBuilder Type<TField>(TypeMatch typeMatch = TypeMatch.Exact);
    
    TBuilder Modifiers(FieldModifiers modifiers);
}

internal class FieldCriterionBuilder<TBuilder> :
    MemberCriterionBuilder<TBuilder, IFieldCriterion, FieldInfo>,
    IFieldCriterionBuilder<TBuilder>
    where TBuilder : IFieldCriterionBuilder<TBuilder>
{
    protected FieldCriterionBuilder(IFieldCriterion criterion) : base(criterion)
    {
    }

    public TBuilder Type(ICriterion<Type> criterion)
    {
        _criterion.Type = criterion;
        return _builder;
    }
    public TBuilder Type(Type type, TypeMatch typeMatch = TypeMatch.Exact)
        => Type(new TypeMatchCriterion(type, typeMatch));
    public TBuilder Type<TField>(TypeMatch typeMatch = TypeMatch.Exact)
        => Type(new TypeMatchCriterion(typeof(TField), typeMatch));
    public TBuilder Modifiers(FieldModifiers modifiers)
    {
        _criterion.Modifiers = modifiers;
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