
using ScrubJay.Reflection.IL;
using ScrubJay.Reflection.IL.Emission;

namespace ScrubJay.Reflection.Runtime;

[PublicAPI]
public abstract class Argument : 
    IEquatable<Argument>,
    IRenderable
{
    public static implicit operator Argument(FieldInfo field) => new FieldArgument(field);
    public static implicit operator Argument(ILLocal local) => new LocalArgument(local);
    public static implicit operator Argument(ParameterInfo param) => new ParameterArgument(param);
    public static implicit operator Argument(Type type) => new StackArgument(type);
    
    public required Type Type { get; init; }
    
    protected Argument() { }
    
    [SetsRequiredMembers]
    protected Argument(Type type)
    {
        Type = type;
    }

    public abstract Emitter Load(Emitter emitter);

    public abstract Emitter LoadAddr(Emitter emitter);

    public abstract Emitter Store(Emitter emitter);
    
    public abstract bool Equals(Argument? other);

    public abstract void RenderTo<B>(B builder)
        where B : TextBuilderBase<B>;
    
    
    public bool IsByRef() => Type.IsByRef;
    
    public bool IsByRef(out Type rawType)
    {
        if (Type.IsByRef)
        {
            rawType = Type.GetElementType().ThrowIfNull();
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

    public override sealed bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Argument argument)
            return Equals(argument);
        return false;
    }
    
    
    public override sealed string ToString()
    {
        return TextBuilder.Build(RenderTo);
    }
}