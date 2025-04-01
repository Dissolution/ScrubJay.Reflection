using System.Linq.Expressions;

namespace ScrubJay.Reflection.Searching;

public class Mirror
{
    public static Mirror For(Type type) => new(type);
    public static Mirror<T> For<T>() => new();

    public static MemberInfo? MemberFrom<TMember>(Expression expression)
        where TMember : MemberInfo
    {
        TMember? member = null;
        if (expression is MemberExpression memberExpression && memberExpression.Member.Is(out member))
            return member;
        if (expression is NewExpression newExpression && newExpression.Constructor.Is(out member))
            return member;
        if (expression is LambdaExpression lambdaExpression)
            return MemberFrom<TMember>(lambdaExpression.Body);
        return member;
    }

    protected readonly Type _type;
    private MemberInfo[]? _allMemberInfos;

    protected IReadOnlyList<MemberInfo> AllMemberInfos => _allMemberInfos ??= _type.AllMembers();

    public MemberInfoFilters Members => new(AllMemberInfos);

    public FieldFilters Fields => new(AllMemberInfos.OfType<FieldInfo>());

    public PropertyFilters Properties => new(AllMemberInfos.OfType<PropertyInfo>());

    public EventFilters Events => new(AllMemberInfos.OfType<EventInfo>());

    public MethodFilters Methods => new(AllMemberInfos.OfType<MethodInfo>());

    public ConstructorFilters Constructors => new(AllMemberInfos.OfType<ConstructorInfo>());

    public TypeFilters Types => new(AllMemberInfos.OfType<Type>());


    public Mirror(Type type)
    {
        _type = type.ThrowIfNull();
    }


}

public sealed class Mirror<T> : Mirror
{
    public Mirror() : base(typeof(T))
    {

    }
}