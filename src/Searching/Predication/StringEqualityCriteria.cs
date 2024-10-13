namespace ScrubJay.Reflection.Searching.Predication;

public sealed class StringEqualityCriteria : EqualityCriteria<string?>
{
    public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;
    public StringMatchType StringMatchType { get; set; } = StringMatchType.Exact;

    [SetsRequiredMembers]
    public StringEqualityCriteria(string? value) : base(value)
    {
        
    }
 
    public override bool Matches(string? str)
    {
        if (StringMatchType == StringMatchType.Exact)
        {
            return string.Equals(str!, this.Value!, this.StringComparison);
        }
        
        // null values cannot match after this point
        if (str is null || this.Value is null) return false;

        if (StringMatchType.HasFlags(StringMatchType.Contains))
        {
            if (str.AsSpan().Contains(this.Value.AsSpan(), this.StringComparison))
                return true;
        }
        else
        {
            if (StringMatchType.HasFlags(StringMatchType.StartsWith) && str.StartsWith(this.Value, this.StringComparison))
                return true;
            if (StringMatchType.HasFlags(StringMatchType.EndsWith) && str.EndsWith(this.Value, this.StringComparison))
                return true;
        }

        // does not meet specifications
        return false;
    }
}