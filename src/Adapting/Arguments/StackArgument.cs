using ScrubJay.Reflection.IL.Emission;

namespace ScrubJay.Reflection.Adapting.Arguments;

[PublicAPI]
public sealed class StackArgument : Argument,
#if NET7_0_OR_GREATER
    IEqualityOperators<StackArgument, StackArgument, bool>,
#endif
    IEquatable<StackArgument>
{
    public static implicit operator StackArgument(Type type) => new(type);

    public static bool operator ==(StackArgument? left, StackArgument? right)
        => Equate.EquatableValues<StackArgument>(left, right);

    public static bool operator !=(StackArgument? left, StackArgument? right)
        => !Equate.EquatableValues<StackArgument>(left, right);

    
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

        // value is already on the stack
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

    public bool Equals(StackArgument? other) => other?.Type == Type;

    public override bool Equals(Argument? other) => other is StackArgument stackArg && Equals(stackArg);

    public override bool Equals(object? obj) => obj is StackArgument stackArg && Equals(stackArg);

    public override int GetHashCode() => Hasher.HashMany(typeof(StackArgument), Type);

    public override void RenderTo(TextBuilder builder)
        => builder.Render(Type);
}