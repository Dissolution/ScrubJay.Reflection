using ScrubJay.Reflection.Cloning;

namespace ScrubJay.Reflection.Searching.Criteria;

public abstract record class Criteria
{
    
}

public abstract record class Criteria<T> : Criteria, ICriteria<T>
{
    // The assumption for a criteria is that it always passes.
    // Restrictions are added (reductive)
    public abstract bool Matches(T? value);
}