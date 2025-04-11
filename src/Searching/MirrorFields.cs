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
        return Where(field => field.FieldType == type);
    }

    public B Returning(Type type, TypeMatch match)
    {
        return Where(field => field.FieldType.Matches(type, match));
    }

    public B Returning<T>() => Returning(typeof(T));
    
    public B Returning<T>(TypeMatch match) => Returning(typeof(T), match);
}