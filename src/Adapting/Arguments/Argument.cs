using ScrubJay.Reflection.IL.Emission;
using ScrubJay.Reflection.Validation;

namespace ScrubJay.Reflection.Adapting.Arguments;

[PublicAPI]
public abstract class Argument :
#if NET7_0_OR_GREATER
    IEqualityOperators<Argument, Argument, bool>,
#endif
    IEquatable<Argument>,
    IRenderable
{
    public static implicit operator Argument(FieldInfo field) => new FieldArgument(field);
    public static implicit operator Argument(ILLocal local) => new LocalArgument(local);
    public static implicit operator Argument(ParameterInfo param) => new ParameterArgument(param);
    public static implicit operator Argument(Type type) => new StackArgument(type);

    public static bool operator ==(Argument? left, Argument? right) => Equate.EquatableValues<Argument>(left, right);
    public static bool operator !=(Argument? left, Argument? right) => !Equate.EquatableValues<Argument>(left, right);

    public Type Type { get; }

    protected Argument(Type type)
    {
        MemberAssert.IsInstance(type);
        Type = type;
    }

    public void Deconstruct(out bool isByRef, out Type rawType)
    {
        if (Type.IsByRef)
        {
            isByRef = true;
            rawType = Type.GetElementType()!;
        }
        else
        {
            isByRef = false;
            rawType = Type;
        }
    }


    public abstract Emitter Load(Emitter emitter);

    public abstract Emitter LoadAddr(Emitter emitter);

    public abstract Emitter Store(Emitter emitter);

    public abstract bool Equals(Argument? other);

    public abstract void RenderTo(TextBuilder builder);

    public bool IsByRef() => Type.IsByRef;

    public bool IsByRef(out Type rawType)
    {
        if (Type.IsByRef)
        {
            rawType = Type.GetElementType()!;
            return true;
        }
        rawType = Type;
        return false;
    }

    public virtual Emitter LoadAsInstance(Emitter emitter)
    {
        // is this arg a ref?
        if (IsByRef(out var rawType))
        {
            if (rawType.IsValueType)
            {
                // we want a ref!
                return Load(emitter);
            }
            else
            {
                // have to deref
                return Load(emitter).Ldobj(rawType);
            }
        }
        else // arg is not a ref
        {
            if (rawType.IsValueType)
            {
                // we want a ref!
                return LoadAddr(emitter);
            }
            else
            {
                return Load(emitter);
            }
        }
    }

    public override bool Equals(object? obj) => obj is Argument arg && Equals(arg);

    public override int GetHashCode() => Hasher.HashMany(GetType(), Type);

    public override sealed string ToString()
    {
        return TextBuilder.Build(RenderTo);
    }
}