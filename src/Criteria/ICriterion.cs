using ScrubJay;

namespace Scratch.Criteria;

public interface ICriteria
{

}

public delegate Result<Ok, Exception> CriteriaMatches<in T>(T value);
public delegate Result<Ok, Exception> StatefulCriteriaMatches<in TState, in T>(TState state, T value);


public interface ICriteria<in T> : ICriteria
{
    Result<Ok, Exception> Matches(T value);
}

