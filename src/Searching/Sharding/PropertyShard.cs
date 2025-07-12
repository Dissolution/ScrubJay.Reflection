
using ScrubJay.Reflection.Collections;

namespace ScrubJay.Reflection.Searching.Sharding;

[PublicAPI]
public enum PropertySetModifier
{
    [RenderAs("set")] Set,
    [RenderAs("init")] Init,
    [RenderAs("")] Ctor,
}

public abstract class PropertyShard<S> : MemberShard<S, PropertyInfo>
    where S : PropertyShard<S>
{
    protected PropertyShard(DLCL<PropertyInfo> properties, List<string> filters) : base(properties, filters)
    {
    }

#region Containing

    public S Containing(Type type)
    {
        return AddFilter(property => property.PropertyType == type, $"PropertyType == {type:@}");
    }

    public S Containing(Type type, TypeMatch match)
    {
        return AddFilter(property => property.PropertyType.Matches(type, match), $"PropertyType {match:@} {type:@}");
    }

    public S Containing<T>() => Containing(typeof(T));

    public S Containing<T>(TypeMatch match) => Containing(typeof(T), match);

#endregion

#region Gettable

    public S Gettable()
    {
        return AddFilter(static property => property.GetMethod is not null,
            "is gettable");
    }

    public S Gettable(bool gettable)
    {
        return AddFilter(property => (property.GetMethod is not null) == gettable,
            $"is{(gettable ? " " : " not ")}gettable");
    }

    public S Gettable(Viz visibility)
    {
        return AddFilter(property => property.GetMethod is not null &&
                                     property.GetMethod.Visibility().HasFlags(visibility),
            $"getter is {visibility:@}");
    }

#endregion

#region Settable

    public S Settable()
    {
        return AddFilter(static property => property.SetMethod is not null,
            "is settable");
    }

    public S Settable(bool settable)
    {
        return AddFilter(property => (property.SetMethod is not null) == settable,
            $"is{(settable ? " " : " not ")}settable");
    }

    public S Settable(Viz visibility)
    {
        return AddFilter(property => property.SetMethod is not null &&
                                     property.SetMethod.Visibility().HasFlags(visibility),
            $"setter is {visibility:@}");
    }

    public S Settable(PropertySetModifier modifier)
    {
        return AddFilter(property =>
            {
                return modifier switch
                {
                    PropertySetModifier.Set => property.SetMethod is not null,
                    PropertySetModifier.Init => property.IsInitOnly(),
                    PropertySetModifier.Ctor => property.SetMethod is null,
                    _ => throw InvalidEnumException.Create(modifier),
                };
            }, $"setter is {modifier:@}");
    }

#endregion

#region Indexers

    public S NonIndexers()
    {
        return AddFilter(static property => property.GetIndexParameters().Length == 0, "is not indexer");
    }

    public S IsIndexer()
    {
        return AddFilter(static property => property.GetIndexParameters().Length > 0, "is indexer");
    }

    public S IsIndexer(params Type[]? types)
    {
        if (types is null) return NonIndexers();
        int count = types.Length;
        if (count == 0) return NonIndexers();

        string info = TextBuilder.New
            .Append("is indexer<")
            .EnumerateAndDelimit(types, static (tb, type) => tb.Render(type), ", ")
            .Append('>')
            .ToStringAndDispose();

        return AddFilter(property =>
        {
            var indexerParams = property.GetIndexParameters();
            if (indexerParams.Length != count) return false;
            for (int i = 0; i < count; i++)
            {
                if (indexerParams[i].ParameterType != types[i])
                    return false;
            }

            return true;
        }, info);
    }

    public S IsIndexer(Type[]? types, TypeMatch typeMatch)
    {
        if (types is null) return NonIndexers();
        int count = types.Length;
        if (count == 0) return NonIndexers();

        string info = TextBuilder.New
            .Append("is ")
            .Render(typeMatch)
            .Append(" indexer<")
            .EnumerateAndDelimit(types, static (tb, type) => tb.Render(type), ", ")
            .Append('>')
            .ToStringAndDispose();

        return AddFilter(property =>
        {
            var indexerParams = property.GetIndexParameters();
            if (indexerParams.Length != count) return false;
            for (int i = 0; i < count; i++)
            {
                if (!indexerParams[i].ParameterType.Matches(types[i], typeMatch))
                    return false;
            }

            return true;
        }, info);
    }
    
    
    public S IsIndexer(Type[]? types, TypeMatch[] typeMatches)
    {
        if (types is null) return NonIndexers();
        int count = types.Length;
        if (count == 0) return NonIndexers();

        if (typeMatches.Length != count)
            throw new ArgumentException(
                "If you specify more than one TypeMatch, it must be the same number as the Types",
                nameof(typeMatches));

        string info = TextBuilder.New
            .Append("is indexer<")
            .EnumerateAndDelimit(Enumerable.Range(0,count),
                (tb, i) => tb.Append($"({typeMatches[i]:@} {types[i]:@})"),
                ", ")
             .Append('>')
            .ToStringAndDispose();
        
        return AddFilter(property =>
        {
            var indexerParams = property.GetIndexParameters();
            if (indexerParams.Length != count) return false;
            for (int i = 0; i < count; i++)
            {
                if (!indexerParams[i].ParameterType.Matches(types[i], typeMatches[i]))
                    return false;
            }

            return true;
        }, info);
    }

    public S IsIndexer(Type[]? types, Values<TypeMatch> typeMatches)
    {
        return typeMatches.Match(
            onEmpty: () => IsIndexer(types),
            onValue: match => IsIndexer(types, match),
            onValues: matches => IsIndexer(types, matches));
    }
    
    public S IsIndexer<T1>(Values<TypeMatch> typeMatches = default)
        => IsIndexer([typeof(T1)], typeMatches);

    public S IsIndexer<T1, T2>(Values<TypeMatch> typeMatches = default)
        => IsIndexer([typeof(T1), typeof(T2)], typeMatches);

    public S IsIndexer<T1, T2, T3>(Values<TypeMatch> typeMatches = default)
        => IsIndexer([typeof(T1), typeof(T2), typeof(T3)], typeMatches);

    public S IsIndexer<T1, T2, T3, T4>(Values<TypeMatch> typeMatches = default)
        => IsIndexer([typeof(T1), typeof(T2), typeof(T3), typeof(T4)], typeMatches);

    public S IsIndexer<T1, T2, T3, T4, T5>(Values<TypeMatch> typeMatches = default)
        => IsIndexer([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)], typeMatches);

#endregion
}

[PublicAPI]
public sealed class PropertyShard : PropertyShard<PropertyShard>
{
    internal PropertyShard(DLCL<PropertyInfo> properties, List<string> filters) : base(properties, filters)
    {
    }
}