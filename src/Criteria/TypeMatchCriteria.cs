using System.Text.RegularExpressions;
using ScrubJay;
using ScrubJay.Comparison;
using ScrubJay.Reflection;
using ScrubJay.Reflection.Searching.Criteria;

namespace Scratch.Criteria;

[Flags]
public enum TypeMatch
{
    Exact = 1 << 0,
    Implements = Exact | 1 << 1,
    ImplementedBy = Exact | 1 << 2,
    
    Any = Exact | Implements | ImplementedBy,
}

public record class TypeMatchCriterion : ICriteria<Type>
{
    public Type? Type { get; set; } = null;
    public TypeMatch Match { get; set; } = TypeMatch.Exact;

    public TypeMatchCriterion() { }
    public TypeMatchCriterion(Type type)
    {
        this.Type = type;
    }
    public TypeMatchCriterion(Type type, TypeMatch typeMatch)
    {
        this.Type = type;
        this.Match = typeMatch;
    }
    
    public Result<Ok, Exception> Matches(Type? type)
    {
        if (Match.HasFlags(TypeMatch.Exact))
        {
            if (type == this.Type)
                return Ok();
        }

        if (type is not null && this.Type is not null)
        {
            if (Match.HasFlags(TypeMatch.Implements))
            {
                if (type.Implements(Type))
                    return Ok();
            }
            
            if (Match.HasFlags(TypeMatch.ImplementedBy))
            {
                if (Type.Implements(type))
                    return Ok();
            }
        }

        return Reflexception.Create<ArgumentException>($"{type} does not {Match} {this.Type}", nameof(type));
    }
}