namespace ScrubJay.Reflection.Searching.Predication.Building;

public abstract class CriteriaBuilder<TBuilder, TCriteria, T> : ICriteria<T>
    where TBuilder : CriteriaBuilder<TBuilder, TCriteria, T>
    where TCriteria : ICriteria<T>
{
    protected readonly TBuilder _builder;
    protected TCriteria _criteria;

    protected CriteriaBuilder(TCriteria criteria)
    {
        _builder = (TBuilder)this;
        _criteria = criteria;
    }

    public bool Matches(T value) => _criteria.Matches(value);
}