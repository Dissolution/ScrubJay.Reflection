namespace ScrubJay.Reflection.Searching;

public abstract class MirrorMemberInfoBuilder<B, M> : MirrorMemberBaseBuilder<B, M>
    where B : MirrorMemberInfoBuilder<B, M>
    where M : MemberInfo
{
    public MirrorFields Fields => new(ReflectedType, _values.OfType<FieldInfo>());
    public MirrorProperties Properties => new(ReflectedType, _values.OfType<PropertyInfo>());
    public MirrorEvents Events => new(ReflectedType, _values.OfType<EventInfo>());
    public MirrorConstructors Constructors => new(ReflectedType, _values.OfType<ConstructorInfo>());
    public MirrorMethods Methods => new(ReflectedType, _values.OfType<MethodInfo>());
    public MirrorMethodBases MethodBases => new(ReflectedType, _values.OfType<MethodBase>());
    public MirrorTypes Types => new(ReflectedType, _values.OfType<Type>());
    
    protected MirrorMemberInfoBuilder(Type reflectedType, IEnumerable<M> members) 
        : base(reflectedType, members)
    {
    }

    public B MemberType(MemberTypes memberType)
    {
        if (memberType.FlagCount() != 1)
            throw new ArgumentOutOfRangeException(nameof(memberType), memberType, "This method only accepts a single MemberTypes flag");
        return Only(memberType, (member, mt) => member.MemberType == mt);
    }
    
    public B MemberTypes(MemberTypes memberTypes)
    {
        return Only(memberTypes, (member, mt) => member.MemberType.HasAnyFlags(memberTypes));
    }
}