namespace ScrubJay.Reflection.Searching;

public record class MemberCriteria
{
    public Visibility Visibility { get; set; } = Visibility.Any;
    public Access Access { get; set; } = Access.Any;
    public MemberTypes MemberType { get; set; } = MemberTypes.All;

    public string? Name { get; set; } = null;
    public StringComparison NameComparison { get; set; } = StringComparison.Ordinal;
    public NameCriteria NameCriteria { get; set; } = NameCriteria.Exact;
    
    internal BindingFlags BindingFlags
    {
        get
        {
            BindingFlags flags = default;
            if (Visibility.HasFlag(Visibility.Private))
                flags |= BindingFlags.NonPublic;
            if (Visibility.HasFlag(Visibility.Protected))
                flags |= BindingFlags.NonPublic;
            if (Visibility.HasFlag(Visibility.Internal))
                flags |= BindingFlags.NonPublic;
            if (Visibility.HasFlag(Visibility.Public))
                flags |= BindingFlags.Public;
            if (Access.HasFlag(Access.Static))
                flags |= BindingFlags.Static;
            if (Access.HasFlag(Access.Instance))
                flags |= BindingFlags.Instance;
            return flags;
        }
    }
}