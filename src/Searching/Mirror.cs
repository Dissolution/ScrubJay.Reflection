using System.Linq.Expressions;
using ScrubJay.Reflection.Expressions;

namespace ScrubJay.Reflection.Searching;

public class Mirror : MirrorMemberInfoBuilder<Mirror, MemberInfo>
{
    public static Mirror Reflect(Type type) => new Mirror(type);
    public static Mirror<T> Reflect<T>() => new Mirror<T>();

    public static Mirror<T> In<T>(Expression<Action<T>> expression)
    {
        var members = ExpressionHelper.ExtractMembers(expression)
            .Where(member => member.DeclaringType == typeof(T));
        return new Mirror<T>(members);
    }

    internal Mirror(Type reflectedType, IEnumerable<MemberInfo> members)
        : base(reflectedType, members)
    {
    }

    public Mirror(Type type)
        : base(type, type.AllMembers())
    {
    }
}

public class Mirror<T> : Mirror
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    internal Mirror(IEnumerable<MemberInfo> members) : base(typeof(T), members)
    {
    }

    public Mirror() : base(typeof(T)) { }
}