using System.Linq.Expressions;

namespace ScrubJay.Reflection.Expressions;

public static class PredicateExpression
{
    /// <summary>
    /// Creates a predicate that evaluates to true.
    /// </summary>
    public static Expression<Func<T, bool>> True<T>() => _ => true;

    /// <summary>
    /// Creates a predicate that evaluates to false.
    /// </summary>
    public static Expression<Func<T, bool>> False<T>() => _ => false;

    /// <summary>
    /// Creates a predicate expression from the specified lambda expression.
    /// </summary>
    public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate) => predicate;

    public static Expression<Func<T, bool>> Create<T>(Func<T, bool> predicate) => t => predicate(t);
}

public static class PredicateExpression<T>
{
    /// <summary>
    /// Creates a predicate that evaluates to true.
    /// </summary>
    public static Expression<Func<T, bool>> True => _ => true;

    /// <summary>
    /// Creates a predicate that evaluates to false.
    /// </summary>
    public static Expression<Func<T, bool>> False => _ => false;

    /// <summary>
    /// Creates a predicate expression from the specified lambda expression.
    /// </summary>
    public static Expression<Func<T, bool>> Create(Expression<Func<T, bool>> predicate) => predicate;

    public static Expression<Func<T, bool>> Create(Func<T, bool> predicate) => t => predicate(t);
}
