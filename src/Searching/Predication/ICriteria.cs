namespace ScrubJay.Reflection.Searching.Predication;

// marker interface
public interface ICriteria
{

}

public interface ICriteria<in T> : ICriteria
{
    bool Matches(T value);
}

