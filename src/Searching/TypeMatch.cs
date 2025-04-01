namespace ScrubJay.Reflection.Searching;

[Flags]
public enum TypeMatch
{
    Exact = 1 << 0,
    Implements = Exact | 1 << 1,
    ImplementedBy = Exact | 1 << 2,

    Any = Exact | Implements | ImplementedBy,
}