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

    public B Contains(Type type)
    {
        return Only(type, static (field, t) => field.FieldType == t);
    }

    public B Contains(Type type, TypeMatch match)
    {
        return Only(type, match, static (field,t,m) => field.FieldType.Matches(t,m));
    }

    public B Contains<T>() => Contains(typeof(T));
    
    public B Contains<T>(TypeMatch match) => Contains(typeof(T), match);
}