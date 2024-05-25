namespace ScrubJay.Reflection.Searching.Scratch;

public interface ICriterion
{

}

public interface ICriterion<in T> : ICriterion
{
    bool Matches(T? value);
}
