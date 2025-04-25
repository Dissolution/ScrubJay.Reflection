using ScrubJay.Reflection.IL.Emission;

namespace ScrubJay.Reflection.Runtime;

[PublicAPI]
public sealed class ParameterArgument : Argument,
    IEquatable<ParameterArgument>
{
    public static implicit operator ParameterArgument(ParameterInfo param) => new ParameterArgument(param);
    
    public required ParameterInfo Parameter { get; init; }
    
    [SetsRequiredMembers]
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
    
    public override void RenderTo<B>(B builder)
    {
        builder.Render(Parameter);
    }

    public bool Equals(ParameterArgument? other)
        => other is not null &&
            other.Parameter == this.Parameter;

    public override bool Equals(Argument? other)
        => other is ParameterArgument parameterArg &&
            Equals(parameterArg);

}