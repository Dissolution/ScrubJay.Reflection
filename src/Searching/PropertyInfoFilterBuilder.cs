namespace ScrubJay.Reflection.Searching;

public sealed class PropertyFilters : PropertyInfoFilterBuilder<PropertyFilters>
{
    public PropertyFilters(IEnumerable<PropertyInfo> properties) : base(properties)
    {
    }
}

public abstract class PropertyInfoFilterBuilder<TB> : MemberBaseFilterBuilder<TB, PropertyInfo>
    where TB : PropertyInfoFilterBuilder<TB>
{
    public TB HasGetter => Where(p => p.GetMethod is not null);
    public TB HasSetter => Where(p => p.SetMethod is not null);

    public TB HasIndexer => Where(p => p.GetIndexParameters().Length > 0);
    public TB NoIndexers => Where(p => p.GetIndexParameters().Length == 0);

    protected PropertyInfoFilterBuilder(IEnumerable<PropertyInfo> properties) : base(properties)
    {

    }

    public TB ValueType(Type type, TypeMatch match = TypeMatch.Exact)
        => Where(p => p.PropertyType.Equals(type, match));
    public TB ValueType<T>(TypeMatch match = TypeMatch.Exact)
    => Where(p => p.PropertyType.Equals(typeof(T), match));

}
