using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ScrubJay.Reflection.Searching.Scratch;

[Flags]
public enum TypeMatch
{
    Exact = 1 << 0,
    Implements = 1 << 1,
    ImplementedBy = 1 << 2,
    
    Any = Exact | Implements | ImplementedBy,
}

public record class TypeMatchCriterion : ICriterion<Type>
{
    public Type? Type { get; set; } = null;
    public TypeMatch TypeMatch { get; set; } = TypeMatch.Exact;

    public TypeMatchCriterion() { }
    public TypeMatchCriterion(Type type)
    {
        this.Type = type;
    }
    public TypeMatchCriterion(Type type, TypeMatch typeMatch)
    {
        this.Type = type;
        this.TypeMatch = typeMatch;
    }
    
    public bool Matches(Type? str)
    {
        if (Type is null || str is null)
            return Type is null && str is null && TypeMatch.HasFlags(TypeMatch.Exact);
        
        if (TypeMatch.HasFlags(TypeMatch.Exact))
        {
            if (Type == str) return true;
        }
        if (TypeMatch.HasFlags(TypeMatch.Implements))
        {
            if (str.Implements(Type)) return true;
        }
        if (TypeMatch.HasFlags(TypeMatch.ImplementedBy))
        {
            if (Type.Implements(str)) return true;
        }

        return false;
    }
}