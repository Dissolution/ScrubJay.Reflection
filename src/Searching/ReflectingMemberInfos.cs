namespace ScrubJay.Reflection.Searching;

[PublicAPI]
public abstract class ReflectingMemberInfos<B, M> : ReflectingMemberBases<B, M>
    where B : ReflectingMemberInfos<B, M>
    where M : MemberInfo
{
    protected ReflectingMemberInfos(IEnumerable<M> members) 
        : base(members)
    {
    }

    public ReflectingFields Fields() => new (_values.OfType<FieldInfo>());
    public ReflectingFields Fields<T>() => Fields().Containing<T>();
    
    public ReflectingProperties Properties() => new (_values.OfType<PropertyInfo>());
    public ReflectingProperties Properties<T>() => Properties().Containing<T>();

    public ReflectingEvents Events() => new (_values.OfType<EventInfo>());
    
    public MirrorMethodBases MethodBases() => new(_values.OfType<MethodBase>());
    public ReflectingCtors Constructors() => new( _values.OfType<ConstructorInfo>());
    public ReflectingMethods Methods() => new(_values.OfType<MethodInfo>());
    
    public ReflectingTypes Types() => new(_values.OfType<Type>());
    
    
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