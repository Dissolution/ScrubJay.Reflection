using ScrubJay.Text.Comparison;

namespace ScrubJay.Reflection.Searching;

public sealed class MirrorParameters : MirrorParametersBuilder<MirrorParameters>
{
    internal MirrorParameters(IEnumerable<ParameterInfo> values) : base(values)
    {
        
    }
}

public abstract class MirrorParametersBuilder<B> : FluentListBuilder<B, ParameterInfo>
    where B : MirrorParametersBuilder<B>
{
    protected MirrorParametersBuilder(IEnumerable<ParameterInfo> values) : base(values)
    {
        
    }
    
    public B Named(string name)
    {
        return Where(param => TextHelper.Equate(param.Name, name));
    }

    public B Named(string? name, StringMatch match)
    {
        return Where(param => param.Name.Matches(name, match));
    }

    public B With(Type attributeType)
    {
        Throw.IfNull(attributeType);
        if (!attributeType.Implements<Attribute>())
            throw new ArgumentException($"{attributeType.NameOf()} does not implement Attribute", nameof(attributeType));
        return Where(param => param.HasAttribute(attributeType));
    }

    public B With<A>()
        where A : Attribute
    {
        return Where(param => param.HasAttribute<A>());
    }

    public B TypeRefKind(TRK kind)
    {
        return Where(param => param.TypeRefKind() == kind);
    }

    public B OfType(Type? parameterType, TypeMatch match = TypeMatch.Exact)
    {
        return Where(param => param.ParameterType.Matches(parameterType, match));
    }

    public B OfType<P>(TypeMatch match = TypeMatch.Exact) => OfType(typeof(P), match);
}