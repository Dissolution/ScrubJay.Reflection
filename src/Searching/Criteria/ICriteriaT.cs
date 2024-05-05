namespace ScrubJay.Reflection.Searching.Criteria;

public interface ICriteria<in T>
{
    bool Matches(T? value);
}