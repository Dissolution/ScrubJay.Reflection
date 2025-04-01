namespace ScrubJay.Reflection.Searching;

public sealed class MethodFilters : MethodInfoFilterBuilder<MethodFilters>
{
    public MethodFilters(IEnumerable<MethodInfo> methods) : base(methods)
    {
    }
}

public abstract class MethodInfoFilterBuilder<TB> : MethodBaseFilterBuilder<TB, MethodInfo>
    where TB : MethodInfoFilterBuilder<TB>
{
    protected MethodInfoFilterBuilder(IEnumerable<MethodInfo> methods) : base(methods)
    {
    }

    public TB Returning(Type type) => Where(m => m.ReturnType == type);

    public TB Returning<TReturn>() => Returning(typeof(TReturn));
}
