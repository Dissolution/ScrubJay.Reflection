//using ScrubJay.Text.Comparison;
//
//namespace ScrubJay.Reflection.Searching;
//
//public sealed class ReflectingParameters : ReflectingParameterInfos<ReflectingParameters>
//{
//    public ReflectingParameters(IEnumerable<ParameterInfo> parameters) 
//        : base(parameters)
//    {
//        
//    }
//}
//
//public abstract class ReflectingParameterInfos<B> : FluentListBuilder<B, ParameterInfo>
//    where B : ReflectingParameterInfos<B>
//{
//    protected ReflectingParameterInfos(IEnumerable<ParameterInfo> parameters) 
//        : base(parameters)
//    {
//        
//    }
//    
//    public B Named(string name)
//    {
//        return Only(name, static (param,n) => TextHelper.Equate(param.Name, n));
//    }
//
//    public B Named(string? name, StringMatch match)
//    {
//        return Only(name, match, static (param,n,m) => param.Name.Matches(n,m));
//    }
//
//    public B With(Type attributeType)
//    {
//        Throw.IfNull(attributeType);
//        if (!attributeType.Implements<Attribute>())
//            throw new ArgumentException($"{attributeType.NameOf()} does not implement Attribute", nameof(attributeType));
//        return Only(attributeType, static (param,at) => param.HasAttribute(at));
//    }
//
//    public B With<A>()
//        where A : Attribute
//        => With(typeof(A));
//
//    public B TypeRefKind(TRK kind)
//    {
//        return Only(kind, static (param,trk) => param.TypeRefKind() == trk);
//    }
//
//    public B OfType(Type? parameterType, TypeMatch match = TypeMatch.Exact)
//    {
//        return Only(parameterType, match,
//            static (param,pt,m) => param.ParameterType.Matches(pt,m));
//    }
//
//    public B OfType<P>(TypeMatch match = TypeMatch.Exact) => OfType(typeof(P), match);
//}