namespace ScrubJay.Reflection.Searching.Predication.Members;

public sealed class TypeEqualityCriteria : EqualityCriteria<Type>
{
    public TypeMatchType TypeMatch { get; set; } = TypeMatchType.Exact;

    [SetsRequiredMembers]
    public TypeEqualityCriteria(Type type) : base(type)
    {
        
    }
    
    [SetsRequiredMembers]
    public TypeEqualityCriteria(Type type, TypeMatchType typeMatch) : base(type)
    {
        this.TypeMatch = typeMatch;
    }
    
    public override bool Matches(Type? type)
    {
        if (Value is null || type is null)
            return Value is null && type is null && TypeMatch.HasFlags(TypeMatchType.Exact);
        
        if (TypeMatch.HasFlags(TypeMatchType.Exact))
        {
            if (Value == type) return true;
        }
        if (TypeMatch.HasFlags(TypeMatchType.Implements))
        {
            if (type.Implements(Value)) return true;
        }
        if (TypeMatch.HasFlags(TypeMatchType.ImplementedBy))
        {
            if (Value.Implements(type)) return true;
        }

        return false;
    }
}