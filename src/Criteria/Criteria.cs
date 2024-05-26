using ScrubJay;
using ScrubJay.Reflection;

namespace Scratch.Criteria;

public static class Criteria
{
    public static ICriteria<T> Pass<T>() => Criteria<T>.Pass;
    public static ICriteria<T> Fail<T>() => Criteria<T>.Fail;
    public static ICriteria<Option<T>> None<T>() => Criteria<T>.None;
    public static ICriteria<Option<T>> Some<T>() => Criteria<T>.Some;
    public static ICriteria<Option<T>> Some<T>(ICriteria<T> some)
        => new FuncCriteria<Option<T>>(opt => opt.IsSome(out var s) && some.Matches(s));

    public static ICriteria<T> IsNull<T>() => Criteria<T>.IsNull;
    public static ICriteria<T> NotNull<T>() => Criteria<T>.NotNull;


    public static FuncCriteria<T> Any<T>()
        where T : IEnumerable
    {
        return new FuncCriteria<T>(static value =>
        {
            if (value is null) return false;
            IEnumerator? enumerator = default;
            try
            {
                enumerator = value.GetEnumerator();
                return enumerator.MoveNext();
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    ((IDisposable)enumerator).Dispose();
                }
            }
        });
    }

    public static ICriteria<string> Match(string str) => new TextMatchCriteria(str);
    public static ICriteria<string> Match(string str, TextMatch match) => new TextMatchCriteria(str, match);
    public static ICriteria<string> Match(string str, StringComparison comparison) => new TextMatchCriteria(str, comparison);
    public static ICriteria<string> Match(string str, TextMatch match, StringComparison comparison) => new TextMatchCriteria(str, match, comparison);

    public static ICriteria<Type> Match(Type type, TypeMatch match = TypeMatch.Exact) => new TypeMatchCriterion(type, match);

    public static ICriteria<T> Match<T>(T? value, IEqualityComparer<T>? comparer = null)
    {
        return new EqualityMatchCriteria<T>
        {
            Value = value,
            Comparer = comparer,
        };
    }

    public static ICriteria<T[]> Combine<T>(params ICriteria<T>[] criteria)
    {
        return new FuncCriteria<T[]>(array =>
        {
            if (array is null) return false;
            int count = criteria.Length;
            if (array.Length != count) return false;
            for (var i = 0; i < count; i++)
            {
                if (!criteria[i].Matches(array[i]))
                    return false;
            }
            return true;
        });
    }

    public static ICriteria<T> And<T>(params ICriteria<T>[] criteria)
    {
        return new FuncCriteria<T>(value =>
        {
            Result<Ok, Exception> result;
            foreach (ICriteria<T> criterion in criteria)
            {
                result = criterion.Matches(value);
                if (!result)
                    return result;
            }
            return Ok();
        });
    }
    public static ICriteria<T> Or<T>(params ICriteria<T>[] criteria)
    {
        return new FuncCriteria<T>(value =>
        {
            Result<Ok, Exception> result;
            List<Exception> exceptions = new(0);
            foreach (ICriteria<T> criterion in criteria)
            {
                result = criterion.Matches(value);
                if (!result.IsError(out var ex))
                    return result;
                exceptions.Add(ex);
            }
            return new AggregateException(exceptions);
        });
    }
    public static ICriteria<T> Not<T>(ICriteria<T> criterion)
    {
        return new FuncCriteria<T>(value => !criterion.Matches(value));
    }


    public static CriteriaAdapter<TIn, TOut> Adapt<TIn, TOut>(Func<TIn, Result<TOut, Exception>> convert, ICriteria<TOut> outputCriteria)
    {
        return new CriteriaAdapter<TIn, TOut>(convert, outputCriteria);
    }
    
    public static ICriteria<TEnumerable> Contains<TEnumerable, T>(T item, IEqualityComparer<T>? itemComparer = null)
        where TEnumerable : IEnumerable<T>
    {
        return new FuncCriteria<TEnumerable>(items => items.Contains(item, itemComparer));
    }

    internal static CriteriaMatches<T> Match<T>(Func<T, bool> match)
    {
        return new CriteriaMatches<T>(value =>
        {
            if (match(value)) return Ok();
            return Reflexception.Create<ArgumentException>($"{value} does not match", nameof(value));
        });
    }
    internal static StatefulCriteriaMatches<TState, T> Match<TState, T>(Func<TState, T, bool> statefulMatch)
    {
        return new StatefulCriteriaMatches<TState, T>((state, value) =>
        {
            if (statefulMatch(state, value)) return Ok();
            return Reflexception.Create<ArgumentException>($"{value} with state '{state}' does not match", nameof(value));
        });
    }
}

public static class Criteria<T>
{
    public static ICriteria<T> Pass { get; } = new FuncCriteria<T>(static _ => true);
    public static ICriteria<T> Fail { get; } = new FuncCriteria<T>(static _ => false);
    public static ICriteria<Option<T>> Some { get; } = new FuncCriteria<Option<T>>(static opt => opt.IsSome());
    public static ICriteria<Option<T>> None { get; } = new FuncCriteria<Option<T>>(static opt => opt.IsNone());
    public static ICriteria<T> IsNull { get; } = new FuncCriteria<T>(static v => v is null);
    public static ICriteria<T> NotNull { get; } = new FuncCriteria<T>(static v => v is not null);
}