using ScrubJay.Reflection.IL.Emission;

namespace ScrubJay.Reflection.Adapting.Arguments;

[PublicAPI]
public sealed class LocalArgument : Argument,
#if NET7_0_OR_GREATER
    IEqualityOperators<LocalArgument, LocalArgument, bool>,
#endif
    IEquatable<LocalArgument>
{
    public static implicit operator LocalArgument(ILLocal local) => new LocalArgument(local);

    public static bool operator ==(LocalArgument? left, LocalArgument? right)
        => Equate.EquatableValues<LocalArgument>(left, right);

    public static bool operator !=(LocalArgument? left, LocalArgument? right)
        => !Equate.EquatableValues<LocalArgument>(left, right);

    
    public ILLocal Local { get; }

    public LocalArgument(ILLocal local) : base(local.Type)
    {
        Local = local;
    }

    public override Emitter Load(Emitter emitter)
    {
        return emitter.Ldloc(Local);
    }

    public override Emitter LoadAddr(Emitter emitter)
    {
        return emitter.Ldloca(Local);
    }

    public override Emitter Store(Emitter emitter)
    {
        return emitter.Stloc(Local);
    }
    
    public bool Equals(LocalArgument? other) => other is not null && other.Local == Local;

    public override bool Equals(Argument? other) => other is LocalArgument localArg && Equals(localArg);

    public override bool Equals(object? obj) => obj is LocalArgument localArg && Equals(localArg);

    public override int GetHashCode() => Hasher.HashMany(typeof(LocalArgument), Type, Local);
    
    public override void RenderTo(TextBuilder builder) => builder.Render(Local);
}