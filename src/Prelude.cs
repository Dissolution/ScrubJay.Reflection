#pragma warning disable CS8981

global using BF = System.Reflection.BindingFlags;
global using Viz = ScrubJay.Reflection.Visibility;
global using TRK = ScrubJay.Reflection.TypeRefKind;
global using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;
global using text = System.ReadOnlySpan<char>;
using System.Linq.Expressions;
using ScrubJay.Reflection.Searching;
using ScrubJay.Reflection.Searching.Sharding;

namespace ScrubJay.Reflection;

[PublicAPI]
public static class Prelude
{
    public static UnfocusedShard Shard(Type type) => Mirror.Shard(type);

    public static UnfocusedShard Shard<T>() => Mirror.Shard<T>();

    public static UnfocusedShard Shard<T>(Expression<Action<T>> membersExpression)
        => Mirror.Shard<T>(membersExpression);

    public static UnfocusedShard ShardOn<T>(T value) => Mirror.Shard<T>();
    public static UnfocusedShard ShardOn(object obj) => Mirror.Shard(obj.GetType());

}