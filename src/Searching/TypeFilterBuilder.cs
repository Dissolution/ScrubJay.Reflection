namespace ScrubJay.Reflection.Searching;

public sealed class TypeFilters : TypeFilterBuilder<TypeFilters>
{
    public TypeFilters(IEnumerable<Type> types) : base(types)
    {
    }
}

public abstract class TypeFilterBuilder<TB> : MemberBaseFilterBuilder<TB, Type>
    where TB : TypeFilterBuilder<TB>
{
    protected TypeFilterBuilder(IEnumerable<Type> types) : base(types)
    {
    }

    public TB GenericTypes(params Type[] genericTypes)
        => Where(t => Sequence.Equal(t.GetGenericArguments(), genericTypes));
}
