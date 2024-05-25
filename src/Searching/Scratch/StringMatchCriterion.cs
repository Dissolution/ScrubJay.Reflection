namespace ScrubJay.Reflection.Searching.Scratch;

[Flags]
public enum StringMatch
{
    Exact = 0,
    StartsWith = 1 << 0,
    EndsWith = 1 << 1,
    Contains = 1 << 2 | StartsWith | EndsWith,
}

public record class StringMatchCriterion : ICriterion<string>
{
    public string? String { get; set; } = default;
    public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;
    public StringMatch StringMatch { get; set; } = StringMatch.Exact;

    public StringMatchCriterion() { }
    
    public StringMatchCriterion(string str)
    {
        this.String = str;
    }
    public StringMatchCriterion(string str, StringComparison stringComparison)
    {
        this.String = str;
        this.StringComparison = stringComparison;
    }
    public StringMatchCriterion(string str, StringMatch stringMatch)
    {
        this.String = str;
        this.StringMatch = stringMatch;
    }

    public bool Matches(string? str)
    {
        if (StringMatch == StringMatch.Exact)
        {
            return string.Equals(str!, this.String!, this.StringComparison);
        }
        
        // null values cannot match after this point
        if (str is null || this.String is null) return false;

        if (StringMatch.HasFlags(StringMatch.Contains))
        {
            if (str.Contains(this.String, this.StringComparison))
                return true;
        }
        else
        {
            if (StringMatch.HasFlags(StringMatch.StartsWith) && str.StartsWith(this.String, this.StringComparison))
                return true;
            if (StringMatch.HasFlags(StringMatch.EndsWith) && str.EndsWith(this.String, this.StringComparison))
                return true;
        }

        // does not meet specifications
        return false;
    }
}