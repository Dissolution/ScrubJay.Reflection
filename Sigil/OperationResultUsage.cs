using ScrubJay.Sigil.Utilities;

namespace ScrubJay.Sigil;

/// <summary>
/// Represents an IL operation, and the subsequent operations that may use it's result.
/// </summary>
public sealed class OperationResultUsage<TDelegateType>
{
    /// <summary>
    /// The operation that is producing a result.
    /// </summary>
    public Operation<TDelegateType> ProducesResult { get; private set; }

    /// <summary>
    /// The operations that may use the result produced by the ProducesResult operation.
    /// </summary>
    public IEnumerable<Operation<TDelegateType>> ResultUsedBy { get; private set; }

    internal IEnumerable<TypeOnStack> TypesProduced { get; private set; }

    internal OperationResultUsage(Operation<TDelegateType> producer, IEnumerable<Operation<TDelegateType>> users, IEnumerable<TypeOnStack> typesProduced)
    {
        ProducesResult = producer;
        ResultUsedBy = users.ToList();
        TypesProduced = typesProduced.ToList();
    }

    /// <summary>
    /// Returns a string representation of this OperationResultUsage.
    /// </summary>
    public override string ToString()
    {
        var users = string.Join(", ", ResultUsedBy.Select(r => r.ToString()).OrderBy(_ => _).ToArray());

        if (users.Length == 0)
        {
            return "(" + ProducesResult + ") result is unused";
        }

        return "(" + ProducesResult + ") result is used by (" + users + ")";
    }
}
