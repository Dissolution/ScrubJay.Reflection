namespace ScrubJay.Reflection.Extensions;

[Flags]
public enum TypeMatch
{
    Exact         = 1 << 0,
    Implements    = Exact | 1 << 1,
    ImplementedBy = Exact | 1 << 2,

    Any = Exact | Implements | ImplementedBy,
}

public static class TypeMatchExtensions
{
    public static bool Matches(this Type? type, Type? other, TypeMatch match)
    {
        if (match.HasFlags(TypeMatch.Exact))
        {
            if (type == other)
                return true;
        }
        if (type is null || other is null)
            return false;

        if (match.HasFlags(TypeMatch.Implements))
        {
            if (type.Implements(other))
                return true;
        }

        if (match.HasFlags(TypeMatch.ImplementedBy))
        {
            if (other.Implements(type))
                return true;
        }

        return false;
    }
}