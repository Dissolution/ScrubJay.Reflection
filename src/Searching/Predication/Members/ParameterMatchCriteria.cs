namespace ScrubJay.Reflection.Searching.Predication.Members;

public sealed record class ParameterMatchCriteria : MatchCriteria<ParameterInfo?>
{
    public ICriteria<string?>? Name { get; set; }
    public ICriteria<Type>? ValueType { get; set; }
    public ReferenceType? RefKind { get; set; } 
    public ICriteria<Option<object?>>? Default { get; set; }
    public ICriteria<Attribute[]>? Attributes { get; set; }

    public ParameterMatchCriteria()
    {
    }

    public ParameterMatchCriteria(ParameterInfo parameter)
    {
        this.Name = Criteria.Equals(parameter.Name);
        this.ValueType = Criteria.Equals(parameter.ParameterType);
        this.RefKind = parameter.RefKind();

        if (parameter.HasDefaultValue)
        {
            var pd = parameter.DefaultValue;
            this.Default = new FuncCriteria<Option<object?>>(hasDefault => hasDefault.IsSome(out var some) && Equate.Objects(some, pd));
        }
        else
        {
            this.Default = new FuncCriteria<Option<object?>>(hasDefault => hasDefault.IsNone());
        }

        this.Attributes = new ArrayEqualityCriteria<Attribute>(Attribute.GetCustomAttributes(parameter));
    }

    public override bool Matches(ParameterInfo? parameter)
    {
        if (parameter is null) 
            return false;
        
        if (!Matches(Name, parameter.Name))
            return false;
        
        if (!HasAnyFlags(RefKind, parameter.RefKind(out var parameterType)))
            return false;

        if (!Matches(ValueType, parameterType))
            return false;
        

        Option<object?> defaultValue = parameter.HasDefaultValue ? 
            Option<object?>.Some(parameter.DefaultValue) : 
            Option<object?>.None();
        if (!Matches(Default, defaultValue))
            return false;
        
        if (!Matches(Attributes, Attribute.GetCustomAttributes(parameter)))
            return false;
        
        return true;
    }
}