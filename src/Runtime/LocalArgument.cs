using ScrubJay.Reflection.IL;
using ScrubJay.Reflection.IL.Emission;

namespace ScrubJay.Reflection.Runtime;

[PublicAPI]
public sealed class LocalArgument : Argument,
    IEquatable<LocalArgument>
{
    public static implicit operator LocalArgument(ILLocal local) => new LocalArgument(local);
    
    public required ILLocal Local { get; init; }

    [SetsRequiredMembers]
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
    
    public override void RenderTo<B>(B builder)
    {
        builder.Render(Local);
    }

    public bool Equals(LocalArgument? other)
    {
        return other is not null &&
            other.Local == Local;
    }

    public override bool Equals(Argument? other)
    {
        return other is LocalArgument localArg &&
            Equals(localArg);
    }
}