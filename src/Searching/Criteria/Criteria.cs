using ScrubJay.Reflection.Cloning;

namespace ScrubJay.Reflection.Searching.Criteria;

public abstract record class Criteria : ICriteria
{
    
}

public abstract record class Criteria<T> : Criteria, ICriteria<T>, ICriteria
{
    // The assumption for a criteria is that it always passes.
    // Restrictions are added (reductive)
    public abstract bool Matches(T? value);
}

public interface ICriteria
{
    
}

public interface ICriteria<in T> : ICriteria
{
    bool Matches(T? value);
}