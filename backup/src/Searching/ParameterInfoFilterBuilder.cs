using ScrubJay.Text.Comparison;


namespace ScrubJay.Reflection.Searching;

public sealed class ParameterFilters : ParameterInfoFilterBuilder<ParameterFilters>
{
    public ParameterFilters(IEnumerable<ParameterInfo> parameters) : base(parameters)
    {
    }
}

public abstract class ParameterInfoFilterBuilder<TB> : FilterBuilder<TB, ParameterInfo>
    where TB : ParameterInfoFilterBuilder<TB>
{

    protected ParameterInfoFilterBuilder(IEnumerable<ParameterInfo> parameters)
        : base(parameters)
    {
    }

    public TB HasAttribute(Type attributeType) => Where(m => Attribute.IsDefined(m, attributeType));

    public TB HasAttribute<TA>()
        where TA : Attribute
        => Where(p => Attribute.IsDefined(p, typeof(TA)));


    public TB Named(string exactName) => Where(p => p.Name == exactName);

    public TB Named(string exactName, StringComparison comparison)
        => Where(p => string.Equals(p.Name, exactName, comparison));

    public TB Named(string exactName, StringMatch match) => Where(p => p.Name.Matches(exactName, match));

    public TB RefKind(ReferenceKind refKind) => Where(p => p.ReferenceKind() == refKind);

    public TB ValueType(Type type, TypeMatch match = TypeMatch.Exact)
        => Where(p => p.ParameterType.Equals(type, match));

}
