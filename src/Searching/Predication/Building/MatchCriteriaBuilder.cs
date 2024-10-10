namespace ScrubJay.Reflection.Searching.Predication.Building;

public class MatchCriteriaBuilder<TBuilder, TCriteria, T> : CriteriaBuilder<TBuilder, TCriteria, T>
    where TBuilder : MatchCriteriaBuilder<TBuilder, TCriteria, T>
    where TCriteria : MatchCriteria<T>
{
    public MatchCriteriaBuilder(TCriteria criteria) : base(criteria)
    {
        
    }

    public TCriteria GetCriteriaCopy() => _criteria with { };
}