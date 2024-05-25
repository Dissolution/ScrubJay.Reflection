namespace ScrubJay.Reflection.Searching.Scratch;

public interface IGenericTypesCriterion : ICriterion
{
    ICriterion<Type>[]? GenericTypes { get; set; }
}