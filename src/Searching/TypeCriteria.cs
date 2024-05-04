namespace ScrubJay.Reflection.Searching;

[Flags]
public enum TypeCriteria
{
    Exact = 1 << 0,
    Implements = 1 << 1,
    ImplementedBy = 1 << 2,
    
    Any = Exact | Implements | ImplementedBy,
}