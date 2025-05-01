using ScrubJay.Reflection.IL.Emission;

namespace ScrubJay.Reflection.Adapting.Arguments;

[PublicAPI]
public sealed class ParameterArgument : Argument,
#if NET7_0_OR_GREATER
    IEqualityOperators<ParameterArgument, ParameterArgument, bool>,
#endif
    IEquatable<ParameterArgument>
{
    public static implicit operator ParameterArgument(ParameterInfo param) => new ParameterArgument(param);

    public static bool operator ==(ParameterArgument? left, ParameterArgument? right)
        => Equate.EquatableValues(left, right);

    public static bool operator !=(ParameterArgument? left, ParameterArgument? right)
        => !Equate.EquatableValues(left, right);


    public ParameterInfo Parameter { get; }
    
    public ParameterArgument(ParameterInfo parameter) : base(parameter.ParameterType)
    {
        Parameter = parameter;
    }

    public override Emitter Load(Emitter emitter)
    {
        return emitter.Ldarg(Parameter.Position);
    }

    public override Emitter LoadAddr(Emitter emitter)
    {
        return emitter.Ldarga(Parameter.Position);
    }

    public override Emitter Store(Emitter emitter)
    {
        return emitter.Starg(Parameter.Position);
    }
    
    public bool Equals(ParameterArgument? other) => other is not null && other.Parameter == this.Parameter;

    public override bool Equals(Argument? other) => other is ParameterArgument parameterArg && Equals(parameterArg);

    public override bool Equals(object? obj) => obj is ParameterArgument parameterArg && Equals(parameterArg);

    public override int GetHashCode() => Hasher.HashMany(typeof(ParameterArgument), Parameter, Type);
    
    public override void RenderTo<B>(B builder) => builder.Render(Parameter);
}