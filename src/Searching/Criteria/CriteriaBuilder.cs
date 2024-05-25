namespace ScrubJay.Reflection.Searching.Criteria;

public abstract class CriteriaBuilder<TBuilder, TCriteria>
    where TBuilder : CriteriaBuilder<TBuilder, TCriteria>
    where TCriteria : Criteria, new()
{
    protected readonly TBuilder _builder;
    protected readonly TCriteria _criteria;
    
    protected CriteriaBuilder()
    {
        _builder = (TBuilder)this;
        _criteria = new TCriteria();
    }
    protected CriteriaBuilder(TCriteria criteria)
    {
        _builder = (TBuilder)this;
        _criteria = criteria;
    }

    public override string ToString() => _criteria.ToString();
}