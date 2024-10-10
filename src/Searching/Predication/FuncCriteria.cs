namespace ScrubJay.Reflection.Searching.Predication;

public sealed class FuncCriteria<T> : ICriteria<T>
{
    private readonly Func<T, bool> _matches;
    
    public FuncCriteria(Func<T, bool> matches)
    {
        _matches = matches;
    }

    public bool Matches(T value) => _matches(value);
}