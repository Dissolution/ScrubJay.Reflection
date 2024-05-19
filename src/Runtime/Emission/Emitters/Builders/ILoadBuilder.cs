namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

public interface ILoadBuilder<out TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    ILoadBuilder<TEmitter> AsAddress { get; }

    TEmitter Argument(int index);
    TEmitter Argument(ParameterInfo parameter);

    TEmitter Local(EmitterLocal local);

    TEmitter Field(FieldInfo field);
    
    TEmitter Token(Type type);
    TEmitter Token(FieldInfo field);
    TEmitter Token(MethodInfo method);

    TEmitter ArrayLength();
    TEmitter ArrayItem(Type itemType);
    TEmitter ArrayItem<TItem>();

    TEmitter FromAddress(Type type);
    TEmitter FromAddress<T>();
}

internal class LoadBuilder<TEmitter> : BuilderBase<TEmitter>, ILoadBuilder<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    private bool _asAddress = false;
    
    public LoadBuilder(Emitter<TEmitter> emitter) : base(emitter)
    {
    }

    public ILoadBuilder<TEmitter> AsAddress
    {
        get
        {
            _asAddress = true;
            return this;
        }
    }
    
    public TEmitter Argument(int index)
    {
        return _asAddress ? _emitter.Ldarga(index) : _emitter.Ldarg(index);
    }
    public TEmitter Argument(ParameterInfo parameter)
    {
        return _asAddress ? _emitter.Ldarga(parameter.Position) : _emitter.Ldarg(parameter.Position);
    }
    public TEmitter Local(EmitterLocal local)
    {
        return _asAddress ? _emitter.Ldloca(local) : _emitter.Ldloc(local);
    }
    public TEmitter Field(FieldInfo field)
    {
        return _asAddress ? _emitter.Ldflda(field) : _emitter.Ldfld(field);
    }
    public TEmitter Token(Type type)
    {
        return _emitter.Ldtoken(type);
    }
    public TEmitter Token(FieldInfo field)
    {
        return _emitter.Ldtoken(field);
    }
    public TEmitter Token(MethodInfo method)
    {
        return _emitter.Ldtoken(method);
    }
    public TEmitter ArrayLength()
    {
        return _emitter.Ldlen();
    }
    public TEmitter ArrayItem(Type itemType)
    {
        return _asAddress ? _emitter.Ldelema(itemType) : _emitter.Ldelem(itemType);
    }
    public TEmitter ArrayItem<TItem>()
    {
        return _asAddress ? _emitter.Ldelema<TItem>() : _emitter.Ldelem<TItem>();
    }
    
    public TEmitter FromAddress(Type type)
    {
        return _emitter.Ldind(type);
    }
    public TEmitter FromAddress<T>()
    {
        return _emitter.Ldind<T>();
    }
}