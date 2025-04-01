namespace ScrubJay.Reflection.Searching;

public sealed class FieldFilters : FieldInfoFilterBuilder<FieldFilters>
{
    public FieldFilters(IEnumerable<FieldInfo> fields) : base(fields)
    {
    }
}

public abstract class FieldInfoFilterBuilder<TB> : MemberBaseFilterBuilder<TB, FieldInfo>
    where TB : FieldInfoFilterBuilder<TB>
{
    protected FieldInfoFilterBuilder(IEnumerable<FieldInfo> fields) : base(fields)
    {

    }

    public TB ValueType(Type type, TypeMatch match = TypeMatch.Exact)
        => Where(f => f.FieldType.Equals(type, match));
    public TB ValueType<T>(TypeMatch match = TypeMatch.Exact)
    => Where(f => f.FieldType.Equals(typeof(T), match));

}
