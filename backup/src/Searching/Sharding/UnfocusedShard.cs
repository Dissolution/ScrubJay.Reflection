

using ScrubJay.Reflection.Collections;

namespace ScrubJay.Reflection.Searching.Sharding;

public abstract class UnfocusedShard<S, M> : MemberShard<S, M>
    where S : UnfocusedShard<S, M>
    where M : MemberInfo
{
    protected UnfocusedShard(DLCL<M> members, List<string> filters) 
        : base(members, filters)
    {
        
    }

    // remain unfocused
    public S MemberType(MemberTypes memberType)
    {
        return AddFilter(
            member => member.MemberType == memberType,
            $"MemberType == {memberType:@}");
    }

    public S MemberTypes(MemberTypes memberTypes)
    {
        return AddFilter(
            member => member.MemberType.HasAnyFlags(memberTypes),
            $"MemberTypes ⊃ {memberTypes:@}");
    }

    public S MemberTypes(params ReadOnlySpan<MemberTypes> memberTypes)
    {
        MemberTypes mt = default;
        foreach (var memberType in memberTypes)
            mt |= memberType;
        return MemberTypes(mt);
    }

    // focus to more specific member type

    public FieldShard Fields()
    {
        var fields = AddTypeFilter<FieldInfo>();
        return new FieldShard(fields, _filters);
    }

    public FieldShard Fields<F>()
    {
        var fields = AddTypeFilter<FieldInfo>();
        return new FieldShard(fields, _filters).Containing<F>();
    }

    public PropertyShard Properties()
    {
        var properties = AddTypeFilter<PropertyInfo>();
        return new PropertyShard(properties, _filters);
    }

    public PropertyShard Properties<P>()
    {
        var properties = AddTypeFilter<PropertyInfo>();
        return new PropertyShard(properties, _filters).Containing<P>();
    }

    public EventShard Events()
    {
        var events = AddTypeFilter<EventInfo>();
        return new EventShard(events, _filters);
    }

    public TypeShard NestedTypes()
    {
        var types = AddTypeFilter<Type>();
        return new TypeShard(types, _filters);
    }

    public MethodBaseShard MethodBases()
    {
        var methods = AddTypeFilter<MethodBase>();
        return new MethodBaseShard(methods, _filters);
    }
    
    public MethodShard Methods()
    {
        var methods = AddTypeFilter<MethodInfo>();
        return new MethodShard(methods, _filters);
    }
    
    public ConstructorShard Constructors()
    {
        var ctors = AddTypeFilter<ConstructorInfo>();
        return new ConstructorShard(ctors, _filters);
    }
}

[PublicAPI]
public sealed class UnfocusedShard : UnfocusedShard<UnfocusedShard, MemberInfo>
{
    // We cache a list of all members of a given type to save time
    private static readonly ConcurrentTypeMap<MemberInfo[]> _allMembersCache = [];
    
    private static MemberInfo[] GetAllMembers(Type type)
    {
        return _allMembersCache.GetOrAdd(type,
            static t => t.GetMembers(
                BF.Public | BF.NonPublic | 
                BF.Instance | BF.Static | 
                BF.FlattenHierarchy | BF.IgnoreCase));

    }
    
    public UnfocusedShard(Type type) 
        : base(DLCL<MemberInfo>.New(GetAllMembers(type)), [])
    {
        // We can show the initial type
        _filters.Add(TextBuilder.New
            .Render(type)
            .Append(" => ")
            .NewLine()
            .ToStringAndDispose());
    }

    public UnfocusedShard(IEnumerable<MemberInfo> members)
        : base(DLCL<MemberInfo>.New(members), [])
    {
        
    }
}