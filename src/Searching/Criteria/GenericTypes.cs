namespace ScrubJay.Reflection.Searching.Criteria;

public interface IGenericTypesCriterion : ICriterion
{
    ICriterion<Type[]> GenericTypes { get; set; }
}
