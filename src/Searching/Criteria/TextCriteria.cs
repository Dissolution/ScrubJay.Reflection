namespace ScrubJay.Reflection.Searching.Criteria;

[Flags]
public enum StringMatch
{
    Exact = 0,
    StartsWith = 1 << 0,
    EndsWith = 1 << 1,
    Contains = 1 << 2 | StartsWith | EndsWith,
}

public record class TextCriteria : Criteria<string>, ICriteria<string>
{
    public static implicit operator TextCriteria(string name) => new TextCriteria() { Text = name };
    
    public string? Text { get; set; } = null;
    public StringMatch Match { get; set; } = StringMatch.Exact;
    public StringComparison Comparison { get; set; } = StringComparison.Ordinal;

    public override bool Matches(string? name)
    {
        // We only do matching if Text is not null, Match and Comparison rely on it
        if (Text is not null)
        {
            if (name is null) return false;

            if (Match == StringMatch.Exact)
            {
                return string.Equals(name, Text, Comparison);
            }
            
            
            if (Match.HasFlags(StringMatch.Contains))
            {
                if (name.Contains(Text, Comparison))
                    return true;
            }
            else
            {
                if (Match.HasFlags(StringMatch.StartsWith) && name.StartsWith(Text, Comparison))
                    return true;
                if (Match.HasFlags(StringMatch.EndsWith) && name.EndsWith(Text, Comparison))
                    return true;
            }

            // does not meet string*
            return false;
        }
        // covers Text == name == null
        return true;
    }
}