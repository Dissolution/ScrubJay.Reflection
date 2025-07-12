using System.Linq.Expressions;
using ScrubJay.Reflection.Expressions;
using ScrubJay.Reflection.Searching.Sharding;

namespace ScrubJay.Reflection.Searching;

[PublicAPI]
public static class Mirror
{
    public static UnfocusedShard Shard(this Type type) => new(type);
    
    public static UnfocusedShard Shard<T>() => new(typeof(T));

    public static UnfocusedShard Shard<T>(Expression<Action<T>> membersExpression)
    {
        return new(membersExpression
            .ExtractMembers()
            .Where(member => member.DeclaringType == typeof(T)));
    }


    public static IEnumerable<M> Members<M>(Expression expression)
        where M : MemberInfo
        => expression.ExtractMembers().OfType<M>();

    public static Result<M> Member<M>(Expression expression)
        where M : MemberInfo
        => expression.ExtractMembers()
            .OfType<M>()
            .TryGetOne();
}