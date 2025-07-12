
using ScrubJay.Reflection.Collections;

namespace ScrubJay.Reflection.Searching.Sharding;

// https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/fields
public abstract class FieldShard<S> : MemberShard<S, FieldInfo>
    where S : FieldShard<S>
{
    protected FieldShard(DLCL<FieldInfo> fields, List<string> filters) : base(fields, filters)
    {
    }
    
    public S Containing(Type type)
    {
        return AddFilter(field => field.FieldType == type, $"FieldType == {type:@}");
    }

    public S Containing(Type type, TypeMatch match)
    {
        return AddFilter(field => field.FieldType.Matches(type, match), $"FieldType {match:@} {type:@}");
    }

    public S Containing<T>() => Containing(typeof(T));
    
    public S Containing<T>(TypeMatch match) => Containing(typeof(T), match);

    public S Readonly()
    {
        return AddFilter(
            static field => field.Attributes.HasFlags(FieldAttributes.InitOnly),
            $"readonly == true");
    }
}

[PublicAPI]
public sealed class FieldShard : FieldShard<FieldShard>
{
    internal FieldShard(DLCL<FieldInfo> fields, List<string> filters) : base(fields, filters)
    {
    }
}