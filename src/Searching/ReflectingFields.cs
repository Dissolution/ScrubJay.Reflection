namespace ScrubJay.Reflection.Searching;

public sealed class ReflectingFields : ReflectingFieldInfos<ReflectingFields>
{
    public ReflectingFields(IEnumerable<FieldInfo> fields) 
        : base(fields)
    {
    }
}

public abstract class ReflectingFieldInfos<B> : ReflectingMemberBases<B, FieldInfo>
    where B : ReflectingFieldInfos<B>
{
    protected ReflectingFieldInfos(IEnumerable<FieldInfo> fields) 
        : base(fields)
    {
    }

    public B Containing(Type type)
    {
        return Only(type, static (field, t) => field.FieldType == t);
    }

    public B Containing(Type type, TypeMatch match)
    {
        return Only(type, match, static (field,t,m) => field.FieldType.Matches(t,m));
    }

    public B Containing<T>() => Containing(typeof(T));
    
    public B Containing<T>(TypeMatch match) => Containing(typeof(T), match);
}