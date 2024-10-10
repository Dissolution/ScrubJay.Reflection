using ScrubJay.Reflection.Searching.Predication.Members;

namespace ScrubJay.Reflection.Searching.Predication;

public abstract class Criteria : ICriteria
{
    public static ICriteria<T> Pass<T>() => Criteria<T>.Pass;
    public static ICriteria<T> Fail<T>() => Criteria<T>.Fail;
    public static ICriteria<T> IsNull<T>() => Criteria<T>.IsNull;
    public static ICriteria<T> IsNotNull<T>() => Criteria<T>.IsNotNull;

    public static ICriteria<TEnumerable> IsNotNullOrEmpty<TEnumerable>()
        where TEnumerable : IEnumerable
    {
        return new FuncCriteria<TEnumerable>(static enumerable =>
        {
            if (enumerable is null) return false;
            var enumerator = enumerable.GetEnumerator();
            try
            {
                return enumerator.MoveNext();
            }
            catch (Exception)
            {
                return false;
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

    public static ICriteria<string?> Equals(
        string? value,
        StringComparison comparison = StringComparison.Ordinal,
        StringMatchType matchType = StringMatchType.Exact)
    {
        return new StringEqualityCriteria(value)
        {
            StringComparison = comparison,
            StringMatchType = matchType,
        };
    }

    public static ICriteria<Type?> Equals(
        Type? type,
        TypeMatchType matchType = TypeMatchType.Exact)
    {
        return new TypeEqualityCriteria(type)
        {
            TypeMatch = matchType,
        };
    }
    
    public static ICriteria<Type?> Equals<T>(TypeMatchType matchType = TypeMatchType.Exact)
    {
        return new TypeEqualityCriteria(typeof(T))
        {
            TypeMatch = matchType,
        };
    }
}

public abstract class Criteria<T> : Criteria, ICriteria<T>
{
    public static ICriteria<T> Pass { get; } = new FuncCriteria<T>(static _ => true);
    public static ICriteria<T> Fail { get; } = new FuncCriteria<T>(static _ => false);
    public static ICriteria<T> IsNull { get; } = new FuncCriteria<T>(static value => value is null);
    public static ICriteria<T> IsNotNull { get; } = new FuncCriteria<T>(static value => value is not null);

    public static ICriteria<T[]> ArrayOfCriteriaToCriteriaOfArray(params ICriteria<T>[] criteria)
    {
        return new FuncCriteria<T[]>(array =>
        {
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
    
    public abstract bool Matches(T value);
}