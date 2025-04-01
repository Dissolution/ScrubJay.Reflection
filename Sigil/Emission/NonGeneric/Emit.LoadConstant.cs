namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Push a 1 onto the stack if b is true, and 0 if false.
    ///
    /// Pushed values are int32s.
    /// </summary>
    public Emit LoadConstant(bool b)
    {
        _innerEmit.LoadConstant(b);
        return this;
    }

    /// <summary>
    /// Push a constant int32 onto the stack.
    /// </summary>
    public Emit LoadConstant(int i)
    {
        _innerEmit.LoadConstant(i);
        return this;
    }

    /// <summary>
    /// Push a constant int32 onto the stack.
    /// </summary>
    public Emit LoadConstant(uint i)
    {
        _innerEmit.LoadConstant(i);
        return this;
    }

    /// <summary>
    /// Push a constant int64 onto the stack.
    /// </summary>
    public Emit LoadConstant(long l)
    {
        _innerEmit.LoadConstant(l);
        return this;
    }

    /// <summary>
    /// Push a constant int64 onto the stack.
    /// </summary>
    public Emit LoadConstant(ulong l)
    {
        _innerEmit.LoadConstant(l);
        return this;
    }

    /// <summary>
    /// Push a constant float onto the stack.
    /// </summary>
    public Emit LoadConstant(float f)
    {
        _innerEmit.LoadConstant(f);
        return this;
    }

    /// <summary>
    /// Push a constant double onto the stack.
    /// </summary>
    public Emit LoadConstant(double d)
    {
        _innerEmit.LoadConstant(d);
        return this;
    }

    /// <summary>
    /// Push a constant string onto the stack.
    /// </summary>
    public Emit LoadConstant(string str)
    {
        _innerEmit.LoadConstant(str);
        return this;
    }

    /// <summary>
    /// Push a constant RuntimeFieldHandle onto the stack.
    /// </summary>
    public Emit LoadConstant(FieldInfo field)
    {
        _innerEmit.LoadConstant(field);
        return this;
    }

    /// <summary>
    /// Push a constant RuntimeMethodHandle onto the stack.
    /// </summary>
    public Emit LoadConstant(MethodInfo method)
    {
        _innerEmit.LoadConstant(method);
        return this;
    }

    /// <summary>
    /// Push a constant RuntimeMethodHandle onto the stack.
    /// </summary>
    public Emit LoadConstant(ConstructorInfo constructor)
    {
        _innerEmit.LoadConstant(constructor);
        return this;
    }

    /// <summary>
    /// Push a constant RuntimeTypeHandle onto the stack.
    /// </summary>
    public Emit LoadConstant<T>()
    {
        _innerEmit.LoadConstant<T>();
        return this;
    }

    /// <summary>
    /// Push a constant RuntimeTypeHandle onto the stack.
    /// </summary>
    public Emit LoadConstant(Type type)
    {
        _innerEmit.LoadConstant(type);
        return this;
    }

    /// <summary>
    /// Loads a null reference onto the stack.
    /// </summary>
    public Emit LoadNull()
    {
        _innerEmit.LoadNull();
        return this;
    }
}
