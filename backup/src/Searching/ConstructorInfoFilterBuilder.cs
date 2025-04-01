namespace ScrubJay.Reflection.Searching;

public sealed class ConstructorFilters : ConstructorInfoFilterBuilder<ConstructorFilters>
{
    public ConstructorFilters(IEnumerable<ConstructorInfo> members) : base(members)
    {
    }
}

public abstract class ConstructorInfoFilterBuilder<TB> : MethodBaseFilterBuilder<TB, ConstructorInfo>
    where TB : ConstructorInfoFilterBuilder<TB>
{
    protected ConstructorInfoFilterBuilder(IEnumerable<ConstructorInfo> constructors) : base(constructors)
    {

    }

    public TB Constructs(Type type, TypeMatch match = TypeMatch.Exact) => Where(m => m.DeclaringType.Equals(type, match));
    public TB Constructs<T>(TypeMatch match = TypeMatch.Exact) => Where(m => m.DeclaringType.Equals(typeof(T), match));
}
