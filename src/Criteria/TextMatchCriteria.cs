using ScrubJay;
using ScrubJay.Comparison;
using ScrubJay.Reflection;

namespace Scratch.Criteria;

[Flags]
public enum TextMatch
{
    Exact = 1 << 0,
    StartsWith = 1 << 1,
    EndsWith = 1 << 2,
    Contains = 1 << 3 | StartsWith | EndsWith | Exact,
}

public sealed record class TextMatchCriteria : ICriteria<string?>, ICriteria<char>, ICriteria<char[]?>
{
    public string? String { get; set; } = null;
    public TextMatch Match { get; set; } = TextMatch.Exact;
    public StringComparison Comparison { get; set; } = StringComparison.Ordinal;

    public TextMatchCriteria() { }
    public TextMatchCriteria(string? str)
    {
        this.String = str;
    }
    public TextMatchCriteria(string? str, TextMatch match)
    {
        this.String = str;
        this.Match = match;
    }
    public TextMatchCriteria(string? str, StringComparison comparison)
    {
        this.String = str;
        this.Comparison = comparison;
    }
    public TextMatchCriteria(string? str, TextMatch match, StringComparison comparison)
    {
        this.String = str;
        this.Match = match;
        this.Comparison = comparison;
    }

    public Result<Ok, Exception> Matches(char value)
    {
        if (String is null)
            return Reflexception.Create<ArgumentException>($"{value} cannot match null", nameof(value));
        if (String.Length != 1)
            return Reflexception.Create<ArgumentException>($"{value} cannot match \"{String}\"", nameof(value));
        if (!string.Equals(value.ToString(), String, Comparison))
            return Reflexception.Create<ArgumentException>($"{value} does not match \"{String}\"", nameof(value));
        return Ok();
    }
    public Result<Ok, Exception> Matches(char[]? value)
    {
        if (Match.HasFlags(TextMatch.Exact))
        {
            if (Relate.Equal.Text(value, String, Comparison))
                return Ok();
        }

        if (value is not null && String is not null &&
            String.Length <= Capture(value.Length, out var len))
        {
            if (Match.HasFlags(TextMatch.StartsWith))
            {
                if (Relate.Equal.Text(value, String[..len], Comparison))
                    return Ok();
            }
            if (Match.HasFlags(TextMatch.EndsWith))
            {
                if (Relate.Equal.Text(value, String[^len..], Comparison))
                    return Ok();
            }
            if (Match.HasFlags(TextMatch.Contains))
            {
                if (String.AsSpan().Contains(value.AsSpan(), Comparison))
                    return Ok();
            }
        }

        return Reflexception.Create<ArgumentException>($"{value} does not {Match} \"{String}\"", nameof(value));
    }
    public Result<Ok, Exception> Matches(string? value)
    {
        if (Match.HasFlags(TextMatch.Exact))
        {
            if (Relate.Equal.Text(value, String, Comparison))
                return Ok();
        }

        if (value is not null && String is not null &&
            String.Length <= Capture(value.Length, out var len))
        {
            if (Match.HasFlags(TextMatch.StartsWith))
            {
                if (Relate.Equal.Text(value, String[..len], Comparison))
                    return Ok();
            }
            if (Match.HasFlags(TextMatch.EndsWith))
            {
                if (Relate.Equal.Text(value, String[^len..], Comparison))
                    return Ok();
            }
            if (Match.HasFlags(TextMatch.Contains))
            {
                if (String.AsSpan().Contains(value.AsSpan(), Comparison))
                    return Ok();
            }
        }

        return Reflexception.Create<ArgumentException>($"{value} does not {Match} \"{String}\"", nameof(value));
    }
}