using ScrubJay.Reflection.IL.Emission;

namespace ScrubJay.Reflection.Runtime;

[PublicAPI]
public sealed class StackArgument : Argument,
    IEquatable<StackArgument>
{
    public static implicit operator StackArgument(Type type) => new(type);
    
    [SetsRequiredMembers]
    public StackArgument(Type type) : base(type)
    {
    }

    public override Emitter Load(Emitter emitter)
    {
        if (IsByRef())
        {
            Debugger.Break();
            // we need to pull the value out of this address
            return emitter.Ldind_Ref();
        }

        // value is already on the the stack
        return emitter;
    }

    public override Emitter LoadAddr(Emitter emitter)
    {
        if (IsByRef())
        {
            // address is on the stack
            return emitter;
        }

        // hack
        Debugger.Break();
        return emitter
            .DeclareLocal(Type, out var temp)
            .Stloc(temp)
            .Ldloca(temp);
    }

    public override Emitter Store(Emitter emitter)
    {
        // value is already on the stack
        return emitter;
    }

    public override void RenderTo<B>(B builder) 
        => builder.AppendType(Type);

    public override bool Equals(Argument? other)
        => other is StackArgument stackArg && Equals(stackArg);

    public bool Equals(StackArgument? other)
        => other?.Type == Type;
}