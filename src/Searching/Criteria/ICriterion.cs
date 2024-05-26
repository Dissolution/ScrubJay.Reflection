namespace ScrubJay.Reflection.Searching.Criteria;

public interface ICriterion
{

}

public interface ICriterion<in T> : ICriterion
{
    bool Matches(T? value);
}


public static class Criterion
{
    public static ICriterion<T> Pass<T>() => Criterion<T>.Pass;
    public static ICriterion<T> Fail<T>() => Criterion<T>.Fail;
    public static ICriterion<Option<T>> None<T>() => Criterion<T>.None;
    public static ICriterion<Option<T>> Some<T>() => Criterion<T>.Some;
    public static ICriterion<Option<T>> Some<T>(ICriterion<T> some) 
        => new FuncCriterion<Option<T>>(opt => opt.IsSome(out var s) && some.Matches(s));

    public static ICriterion<T> IsNull<T>() => Criterion<T>.IsNull;
    public static ICriterion<T> NotNull<T>() => Criterion<T>.NotNull;
    
    
    public static FuncCriterion<T> Any<T>()
        where T : IEnumerable
    {
        return new FuncCriterion<T>(static value =>
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

    public static ICriterion<string> Match(string str)
    {
        return new StringMatchCriterion
        {
            String = str,
        };
    }
    public static ICriterion<string> Match(string str, StringMatch match)
    {
        return new StringMatchCriterion
        {
            String = str,
            StringMatch = match,
        };
    }
    public static ICriterion<string> Match(string str, StringComparison comparison)
    {
        return new StringMatchCriterion
        {
            String = str,
            StringComparison = comparison,
        };
    }
    public static ICriterion<string> Match(string str, StringMatch match, StringComparison comparison)
    {
        return new StringMatchCriterion
        {
            String = str,
            StringMatch = match,
            StringComparison = comparison,
        };
    }

    public static ICriterion<Type> Match(Type type, TypeMatch match = TypeMatch.Exact)
    {
        return new TypeMatchCriterion
        {
            Type = type,
            TypeMatch = match,
        };
    }
    
    public static ICriterion<ParameterInfo> Match(ParameterInfo parameter)
    {
        return new ParameterCriterion()
        {
            RefKind = parameter.RefKind(out var ptype),
            Type = Criterion.Match(ptype),
        };
    }

    public static ICriterion<T> Match<T>(T? value, IEqualityComparer<T>? comparer = null)
    {
        return new ValueMatchCriterion<T>
        {
            Value = value,
            ValueComparer = comparer,
        };
    }

    public static ICriterion<T[]> Combine<T>(params ICriterion<T>[] criteria)
    {
        return new FuncCriterion<T[]>(array =>
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
}

public static class Criterion<T>
{
    public static ICriterion<T> Pass { get; } = new FuncCriterion<T>(static _ => true);
    public static ICriterion<T> Fail { get; } = new FuncCriterion<T>(static _ => false);
    public static ICriterion<Option<T>> Some { get; } = new FuncCriterion<Option<T>>(static opt => opt.IsSome());
    public static ICriterion<Option<T>> None { get; } = new FuncCriterion<Option<T>>(static opt => opt.IsNone());
    public static ICriterion<T> IsNull { get; } = new FuncCriterion<T>(static v => v is null);
    public static ICriterion<T> NotNull { get; } = new FuncCriterion<T>(static v => v is not null);
}

public sealed class FuncCriterion<T> : ICriterion<T>
{
    private readonly Func<T?, bool> _matches;
    
    public FuncCriterion(Func<T?, bool> matches)
    {
        _matches = matches;
    }

    public bool Matches(T? value) => _matches(value);
}
