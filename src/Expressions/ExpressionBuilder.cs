using System.Linq.Expressions;

namespace ScrubJay.Reflection.Expressions;

[PublicAPI]
public abstract class ExpressionBuilder
{
    public static Expression<Func<T, bool>> True<T>() => ExpressionBuilder<T>.True;
    public static Expression<Func<T, bool>> False<T>() => ExpressionBuilder<T>.False;
}

[PublicAPI]
public abstract class ExpressionBuilder<T> : ExpressionBuilder
{
    /// <summary>
    /// Creates a <see cref="Predicate{T}"/> <see cref="Expression"/> that evaluates to <c>true</c>
    /// </summary>
    public static Expression<Func<T, bool>> True { get; } = static value => true;

    /// <summary>
    /// Creates a <see cref="Predicate{T}"/> <see cref="Expression"/> that evaluates to <c>false</c>
    /// </summary>
    public static Expression<Func<T, bool>> False { get; } = static value => false;
}