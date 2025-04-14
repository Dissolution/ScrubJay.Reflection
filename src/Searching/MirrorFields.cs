using ScrubJay.Reflection.Naming;

namespace ScrubJay.Reflection.Searching;

public sealed class MirrorFields : MirrorFieldBuilder<MirrorFields>
{
    internal MirrorFields(Type reflectedType, IEnumerable<FieldInfo> members) 
        : base(reflectedType, members)
    {
    }
}

public abstract class MirrorFieldBuilder<B> : MirrorMemberBaseBuilder<B, FieldInfo>
    where B : MirrorFieldBuilder<B>
{

    protected MirrorFieldBuilder(Type reflectedType, IEnumerable<FieldInfo> members) 
        : base(reflectedType, members)
    {
    }

    public B Returning(Type type)
    {
        return Only(type, static (field, t) => field.FieldType == t);
    }

    public B Returning(Type type, TypeMatch match)
    {
        return Only(type, match, static (field,t,m) => field.FieldType.Matches(t,m));
    }

    public B Returning<T>() => Returning(typeof(T));
    
    public B Returning<T>(TypeMatch match) => Returning(typeof(T), match);
}