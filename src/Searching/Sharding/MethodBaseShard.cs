using ScrubJay.Reflection.Collections;

namespace ScrubJay.Reflection.Searching.Sharding;

[PublicAPI]
public sealed class MethodBaseShard : MethodBaseShard<MethodBaseShard, MethodBase>
{
    internal MethodBaseShard(DLCL<MethodBase> methods, List<string> filters) 
        : base(methods, filters)
    {
    }
}

[PublicAPI]
public abstract class MethodBaseShard<S, M> : MemberShard<S, M>
    where S : MethodBaseShard<S, M>
    where M : MethodBase
{
    protected MethodBaseShard(DLCL<M> methods, List<string> filters)
        : base(methods, filters)
    {
    }

    #region Parameters
    public S NoParameters()
    {
        return AddFilter(
            static method => method.GetParameters().Length == 0,
            "no parameters");
    }

    public S WithParameters()
    {
        return AddFilter(
            static method => method.GetParameters().Length > 0,
            "has parameters");
    }

    public S WithParameters(int count)
    {
        return AddFilter(
            method => method.GetParameters().Length == count,
            $"has {count} parameters");
    }

    public S WithParameters(params Type[]? parameterTypes)
    {
        if (parameterTypes is null) return NoParameters();
        int count = parameterTypes.Length;
        if (count == 0) return NoParameters();

        string info = TextBuilder.New
            .Append("has parameters [")
            .EnumerateAndDelimit(parameterTypes, static (tb, type) => tb.Render(type), ", ")
            .Append(']')
            .ToStringAndDispose();

        return AddFilter(method =>
        {
            var methodParams = method.GetParameters();
            if (methodParams.Length != count) return false;
            for (int i = 0; i < count; i++)
            {
                if (methodParams[i].ParameterType != parameterTypes[i])
                    return false;
            }

            return true;
        }, info);
    }

    public S WithParameters(Type[]? parameterTypes, TypeMatch match)
    {
        if (parameterTypes is null) return NoParameters();
        int count = parameterTypes.Length;
        if (count == 0) return NoParameters();

        string info = TextBuilder.New
            .Append($"has {match:@} parameters [")
            .EnumerateAndDelimit(parameterTypes, static (tb, type) => tb.Render(type), ", ")
            .Append(']')
            .ToStringAndDispose();

        return AddFilter(method =>
        {
            var methodParams = method.GetParameters();
            if (methodParams.Length != count) return false;
            for (int i = 0; i < count; i++)
            {
                if (!methodParams[i].ParameterType.Matches(parameterTypes[i], match))
                    return false;
            }

            return true;
        }, info);
    }

    public S Accepting(params object?[] args)
    {
        int len = args.Length;

        string info = TextBuilder.New
            .Append("parameters can accept [")
            .EnumerateAndDelimit(args, static (tb, type) => tb.Render(type), ", ")
            .Append(']')
            .ToStringAndDispose();

        return AddFilter(method =>
        {
            var methodParams = method.GetParameters();
            var paramCount = methodParams.Length;
            if (paramCount < len)
                return false;

            for (int p = 0; p < paramCount; p++)
            {
                var param = methodParams[p];
                if (p < len)
                {
                    var arg = args[p];
                    if (!param.CanAccept(arg))
                        return false;
                }
                else
                {
                    if (!param.Default().IsSome())
                        return false;
                }
            }

            return true;
        }, info);
    }

    public S WithParameters<T1>() => WithParameters(typeof(T1));
    public S WithParameters<T1, T2>() => WithParameters(typeof(T1), typeof(T2));
    public S WithParameters<T1, T2, T3>() => WithParameters(typeof(T1), typeof(T2), typeof(T3));
    public S WithParameters<T1, T2, T3, T4>() => WithParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4));

    public S WithParameters<T1, T2, T3, T4, T5>() =>
        WithParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
#endregion
#region Generics

    public S IsNotGeneric()
    {
        return AddFilter(static method => method.GetGenericArguments().Length == 0, "is not generic");
    }

    public S AreGeneric()
    {
        return AddFilter(static method => method.GetGenericArguments().Length > 0, "is generic");
    }
    
    public S AreGeneric(int count)
    {
        return AddFilter(method => method.GetGenericArguments().Length == count, $"is generic<{count}>");
    }

    public S AreGeneric(params Type[]? types)
    {
        if (types is null) return IsNotGeneric();
        int count = types.Length;
        if (count == 0) return IsNotGeneric();

        string info = TextBuilder.New
            .Append("is generic<")
            .EnumerateAndDelimit(types, static (tb, type) => tb.Render(type), ", ")
            .Append('>')
            .ToStringAndDispose();

        return AddFilter(method =>
        {
            var genericTypes = method.GetGenericArguments();
            if (genericTypes.Length != count) return false;
            for (int i = 0; i < count; i++)
            {
                if (genericTypes[i] != types[i])
                    return false;
            }

            return true;
        }, info);
    }

    public S AreGeneric(Type[]? types, TypeMatch typeMatch)
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

        return AddFilter(method =>
        {
            var indexerParams = method.GetGenericArguments();
            if (indexerParams.Length != count) return false;
            for (int i = 0; i < count; i++)
            {
                if (!indexerParams[i].Matches(types[i], typeMatch))
                    return false;
            }

            return true;
        }, info);
    }


    public S AreGeneric(Type[]? types, TypeMatch[] typeMatches)
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
            .EnumerateAndDelimit(Enumerable.Range(0, count),
                (tb, i) => tb.Append($"({typeMatches[i]:@} {types[i]:@})"),
                ", ")
            .Append('>')
            .ToStringAndDispose();

        return AddFilter(method =>
        {
            var genericTypes = method.GetGenericArguments();
            if (genericTypes.Length != count) return false;
            for (int i = 0; i < count; i++)
            {
                if (!genericTypes[i].Matches(types[i], typeMatches[i]))
                    return false;
            }

            return true;
        }, info);
    }

    public S AreGeneric(Type[]? types, Values<TypeMatch> typeMatches)
    {
        return typeMatches.Match(
            onEmpty: () => AreGeneric(types),
            onValue: match => AreGeneric(types, match),
            onValues: matches => AreGeneric(types, matches));
    }

    public S AreGeneric<T1>(Values<TypeMatch> typeMatches = default)
        => AreGeneric([typeof(T1)], typeMatches);

    public S AreGeneric<T1, T2>(Values<TypeMatch> typeMatches = default)
        => AreGeneric([typeof(T1), typeof(T2)], typeMatches);

    public S AreGeneric<T1, T2, T3>(Values<TypeMatch> typeMatches = default)
        => AreGeneric([typeof(T1), typeof(T2), typeof(T3)], typeMatches);

    public S AreGeneric<T1, T2, T3, T4>(Values<TypeMatch> typeMatches = default)
        => AreGeneric([typeof(T1), typeof(T2), typeof(T3), typeof(T4)], typeMatches);

    public S AreGeneric<T1, T2, T3, T4, T5>(Values<TypeMatch> typeMatches = default)
        => AreGeneric([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)], typeMatches);

#endregion


    public S Returning(Type type)
    {
        return AddFilter(method => method.ReturnType() == type, $"returns {type:@}");
    }

    public S Returning(Type type, TypeMatch match)
    {
        return AddFilter(method => method.ReturnType().Matches(type, match), $"returns {match:@} {type:@}");
    }

    public S Returning<T>() => Returning(typeof(T));

    public S Returning<T>(TypeMatch match) => Returning(typeof(T), match);
}