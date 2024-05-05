using ScrubJay.Extensions;
using ScrubJay.Validation;

namespace ScrubJay.Reflection.Searching.Criteria;

[Flags]
public enum TypeMatch
{
    Exact = 1 << 0,
    Implements = 1 << 1,
    ImplementedBy = 1 << 2,
    
    Any = Exact | Implements | ImplementedBy,
}

public record class TypeCriteria : MemberCriteria, ICriteria<Type>
{
    public static implicit operator TypeCriteria(Type type) => Create(type);
    
    public static TypeCriteria Create(Type type, TypeMatch match = TypeMatch.Exact)
    {
        Throw.IfNull(type);
        return new()
        {
            Value = type,
            Match = match,
        };
    }
    
    public required Type Value { get; init; }
    public TypeMatch Match { get; init; } = TypeMatch.Exact;

    public override MemberTypes MemberType => MemberTypes.TypeInfo;

    public bool Matches(Type? type)
    {
        if (!base.Matches(type))
            return false;
        
        if (type == Value)
            return Match.HasFlags(TypeMatch.Exact);
       
        if (Match.HasFlags(TypeMatch.Implements) && type.Implements(Value))
            return true;

        if (Match.HasFlags(TypeMatch.ImplementedBy) && Value.Implements(type))
            return true;

        return false;
    }
}