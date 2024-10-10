namespace ScrubJay.Reflection.Searching.Predication.Members;

[Flags]
public enum TypeMatchType
{
    Exact = 1 << 0,
    Implements = Exact | 1 << 1,
    ImplementedBy = Exact | 1 << 2,
    
    Any = Exact | Implements | ImplementedBy,
}