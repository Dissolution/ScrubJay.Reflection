namespace ScrubJay.Reflection.Searching.Predication;

[Flags]
public enum StringMatchType
{
    Exact = 0,
    StartsWith = 1 << 0,
    EndsWith = 1 << 1,
    Contains = 1 << 2 | StartsWith | EndsWith,
}