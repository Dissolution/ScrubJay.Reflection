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
    public MirrorTypes Types => new(ReflectedType, _values.OfType<Type>());
    
    protected MirrorMemberInfoBuilder(Type reflectedType, IEnumerable<M> members) 
        : base(reflectedType, members)
    {
    }
}