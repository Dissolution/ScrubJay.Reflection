
using ScrubJay.Reflection.Collections;

namespace ScrubJay.Reflection.Searching.Sharding;

public abstract class TypeShard<S> : MemberShard<S, Type>
    where S : TypeShard<S>
{
    protected TypeShard(DLCL<Type> types, List<string> filters) : base(types, filters)
    {
    }

  #region Generics

    public S IsNotGeneric()
    {
        return AddFilter(static type => type.GetGenericArguments().Length == 0, "is not generic");
    }

    public S IsGeneric()
    {
        return AddFilter(static type => type.GetGenericArguments().Length > 0, "is generic");
    }
    
    public S IsGeneric(int count)
    {
        return AddFilter(type => type.GetGenericArguments().Length == count, $"is generic<{count}>");
    }

    public S IsGeneric(params Type[]? types)
    {
        if (types is null) return IsNotGeneric();
        int count = types.Length;
        if (count == 0) return IsNotGeneric();

        string info = TextBuilder.New
            .Append("is generic<")
            .EnumerateAndDelimit(types, static (tb, type) => tb.Render(type), ", ")
            .Append('>')
            .ToStringAndDispose();

        return AddFilter(type =>
        {
            var genericTypes = type.GetGenericArguments();
            if (genericTypes.Length != count) return false;
            for (int i = 0; i < count; i++)
            {
                if (genericTypes[i] != types[i])
                    return false;
            }

            return true;
        }, info);
    }

    public S IsGeneric(Type[]? types, TypeMatch typeMatch)
    {
        if (types is null) return IsNotGeneric();
        int count = types.Length;
        if (count == 0) return IsNotGeneric();

        string info = TextBuilder.New
            .Append("is ")
            .Render(typeMatch)
            .Append(" generic<")
            .EnumerateAndDelimit(types, static (tb, type) => tb.Render(type), ", ")
            .Append('>')
            .ToStringAndDispose();

        return AddFilter(type =>
        {
            var indexerParams = type.GetGenericArguments();
            if (indexerParams.Length != count) return false;
            for (int i = 0; i < count; i++)
            {
                if (!indexerParams[i].Matches(types[i], typeMatch))
                    return false;
            }

            return true;
        }, info);
    }
    
    
    public S IsGeneric(Type[]? types, TypeMatch[] typeMatches)
    {
        if (types is null) return IsNotGeneric();
        int count = types.Length;
        if (count == 0) return IsNotGeneric();

        if (typeMatches.Length != count)
            throw new ArgumentException(
                "If you specify more than one TypeMatch, it must be the same number as the Types",
                nameof(typeMatches));

        string info = TextBuilder.New
            .Append("is generic<")
            .EnumerateAndDelimit(Enumerable.Range(0,count),
                (tb, i) => tb.Append($"({typeMatches[i]:@} {types[i]:@})"),
                ", ")
             .Append('>')
            .ToStringAndDispose();
        
        return AddFilter(type =>
        {
            var genericTypes = type.GetGenericArguments();
            if (genericTypes.Length != count) return false;
            for (int i = 0; i < count; i++)
            {
                if (!genericTypes[i].Matches(types[i], typeMatches[i]))
                    return false;
            }

            return true;
        }, info);
    }

    public S IsGeneric(Type[]? types, Values<TypeMatch> typeMatches)
    {
        return typeMatches.Match(
            onEmpty: () => IsGeneric(types),
            onValue: match => IsGeneric(types, match),
            onValues: matches => IsGeneric(types, matches));
    }
    
    public S IsGeneric<T1>(Values<TypeMatch> typeMatches = default)
        => IsGeneric([typeof(T1)], typeMatches);

    public S IsGeneric<T1, T2>(Values<TypeMatch> typeMatches = default)
        => IsGeneric([typeof(T1), typeof(T2)], typeMatches);

    public S IsGeneric<T1, T2, T3>(Values<TypeMatch> typeMatches = default)
        => IsGeneric([typeof(T1), typeof(T2), typeof(T3)], typeMatches);

    public S IsGeneric<T1, T2, T3, T4>(Values<TypeMatch> typeMatches = default)
        => IsGeneric([typeof(T1), typeof(T2), typeof(T3), typeof(T4)], typeMatches);

    public S IsGeneric<T1, T2, T3, T4, T5>(Values<TypeMatch> typeMatches = default)
        => IsGeneric([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)], typeMatches);

#endregion

    public S IsNested()
    {
        return AddFilter(static type => type.IsNested, $"is nested");
    }
}

[PublicAPI]
public sealed class TypeShard : TypeShard<TypeShard>
{
    internal TypeShard(DLCL<Type> types, List<string> filters) : base(types, filters)
    {
    }
}