using ScrubJay;

namespace Scratch.Criteria;

public class CriteriaAdapter<TIn, TOut> : ICriteria<TIn>//, ICriteria<TOut>
{
    private readonly Func<TIn, Result<TOut, Exception>> _convert;
    private readonly ICriteria<TOut> _outputCriteria;

    public CriteriaAdapter(Func<TIn, Result<TOut, Exception>> convert, ICriteria<TOut> outputCriteria)
    {
        _convert = convert;
        _outputCriteria = outputCriteria;
    }

    public Result<Ok, Exception> Matches(TIn input)
    {
        return _convert(input).Match(
            ok => _outputCriteria.Matches(ok),
            err => err); 
    }

    public Result<Ok, Exception> Matches(TOut output)
    {
        return _outputCriteria.Matches(output);
    }
}