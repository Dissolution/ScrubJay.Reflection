using ScrubJay.Reflection.Extensions;
using ScrubJay.Validation;

namespace ScrubJay.Reflection.Searching.Criteria;

[Flags]
public enum StringMatch
{
    Exact = 0,
    StartsWith = 1 << 0,
    EndsWith = 1 << 1,
    Contains = 1 << 2 | StartsWith | EndsWith,
}

public record class NameCriteria : Criteria, ICriteria<string>
{
    public static implicit operator NameCriteria(string name) => Create(name);
    
    public static NameCriteria Create(string name, StringMatch match = StringMatch.Exact, StringComparison comparison = StringComparison.Ordinal)
    {
        Throw.IfNull(name);
        return new()
        {
            Value = name,
            Match = match,
            Comparison = comparison,
        };
    }

    public required string? Value { get; set; }
    public StringMatch Match { get; set; } = StringMatch.Exact;
    public StringComparison Comparison { get; set; } = StringComparison.Ordinal;

    public bool Matches(string? name)
    {
        if (name is null || Value is null)
        {
            return Match == StringMatch.Exact &&
                name is null &&
                Value is null;
        }
        
        if (Match == StringMatch.Exact)
        {
            if (string.Equals(name, Value, Comparison))
                return true;
        }
        else if (Match.HasFlags(StringMatch.Contains))
        {
            if (name.Contains(Value, Comparison))
                return true;
        }
        else
        {
            if (Match.HasFlags(StringMatch.StartsWith) && name.StartsWith(Value, Comparison))
                return true;
            if (Match.HasFlags(StringMatch.EndsWith) && name.EndsWith(Value, Comparison))
                return true;
        }
        return false;
    }
}