using ScrubJay.Reflection.IL.Instructions;
using ScrubJay.Reflection.IL.LabelOffSetManagement;

namespace ScrubJay.Reflection.IL.Emission;

[PublicAPI]
public interface ICleanEmitter<E> : IEmitter<E>
    where E : ICleanEmitter<E>
{
    E Math(MathOp op, bool overflowCheck = false, bool unsigned = false);
    E Bitwise(BitwiseOp op);

    E Call(MethodBase method);
    
    E Branch(CompareOp op, ILLabel label, bool unsigned = false);
    E Branch(bool boolean, ILLabel label);
    E Scoped(Action<E> scopedBlock);
    
    E Compare(CompareOp op, bool unsigned = false);

    E Push<T>(T value);
    E PushDefault<T>();
    E PushDefaultAddr<T>();

    E LoadArg(int index);
    E LoadArg(ParameterInfo parameter);
    E LoadArgAddr(int index);
    E LoadArgAddr(ParameterInfo parameter);
    E StoreArg(int index);
    E StoreArg(ParameterInfo parameter);
    
    E Declare<T>(out ILLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);
    E LoadLocal(ILLocal local);
    E LoadLocal(int index);
    E LoadLocalAddr(ILLocal local);
    E LoadLocalAddr(int index);
    E StoreLocal(ILLocal local);
    E StoreLocal(int index);

    E LoadElement<T>();
    E LoadElementAddr<T>();
    E StoreElement<T>();

    E LoadField(FieldInfo field);
    E LoadFieldAddr(FieldInfo field);
    E StoreField(FieldInfo field);

    E LoadIndirect<T>();
    E StoreIndirect<T>();
    
    E Define(out ILLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);
    E Mark(ILLabel label);
    E Mark(out ILLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);

    E Return();
    E Box<T>();
    E Castclass<C>()
        where C : class;
    E Unbox<T>();
    E BoxIfNeeded<T>();
    
    E Conv<T>(bool overflowCheck = false, bool unsigned = false);
    
    
    // try/catch/finally
    ITryCatchFinally<E> Try(Action<E, ILLabel> tryBlock);
    E Leave(ILLabel label);

    E Throw();
    E ThrowNew<X>(params object?[] args)
        where X : Exception;



}


public class CleanEmitter : CleanEmitterBase<CleanEmitter, Emitter>,
    ICleanEmitter<CleanEmitter>
{
    public Emitter Simple => _emitter;
    
    public CleanEmitter(Emitter emitter) : base(emitter)
    {
        
    }
}


public class CleanEmitterBase<E, W> : BuilderBase<E>, ICleanEmitter<E>
    where E : CleanEmitterBase<E, W>
    where W : ISimpleEmitter<W>
{
    protected readonly W _emitter;
    
    protected internal CleanEmitterBase(W emitter)
    {
        _emitter = emitter;
    }

    public IInstructions Instructions => _emitter.Instructions;
    
    
    public E Math(MathOp op, bool overflowCheck = false, bool unsigned = false) => throw new NotImplementedException();

    public E Bitwise(BitwiseOp op) => throw new NotImplementedException();

    public E Call(MethodBase method) => throw new NotImplementedException();

    public E Branch(CompareOp op, ILLabel label, bool unsigned = false) => throw new NotImplementedException();

    public E Branch(bool boolean, ILLabel label) => throw new NotImplementedException();

    public E Scoped(Action<E> scopedBlock) => throw new NotImplementedException();

    public E Compare(CompareOp op, bool unsigned = false) => throw new NotImplementedException();

    public E Push<T>(T value) => throw new NotImplementedException();

    public E PushDefault<T>() => throw new NotImplementedException();

    public E PushDefaultAddr<T>() => throw new NotImplementedException();

    public E LoadArg(int index) => throw new NotImplementedException();

    public E LoadArg(ParameterInfo parameter) => throw new NotImplementedException();

    public E LoadArgAddr(int index) => throw new NotImplementedException();

    public E LoadArgAddr(ParameterInfo parameter) => throw new NotImplementedException();

    public E StoreArg(int index) => throw new NotImplementedException();

    public E StoreArg(ParameterInfo parameter) => throw new NotImplementedException();

    public E Declare<T>(out ILLocal local, string? localName = null) => throw new NotImplementedException();

    public E LoadLocal(ILLocal local) => throw new NotImplementedException();

    public E LoadLocal(int index) => throw new NotImplementedException();

    public E LoadLocalAddr(ILLocal local) => throw new NotImplementedException();

    public E LoadLocalAddr(int index) => throw new NotImplementedException();

    public E StoreLocal(ILLocal local) => throw new NotImplementedException();

    public E StoreLocal(int index) => throw new NotImplementedException();

    public E LoadElement<T>() => throw new NotImplementedException();

    public E LoadElementAddr<T>() => throw new NotImplementedException();

    public E StoreElement<T>() => throw new NotImplementedException();

    public E LoadField(FieldInfo field) => throw new NotImplementedException();

    public E LoadFieldAddr(FieldInfo field) => throw new NotImplementedException();

    public E StoreField(FieldInfo field) => throw new NotImplementedException();

    public E LoadIndirect<T>() => throw new NotImplementedException();

    public E StoreIndirect<T>() => throw new NotImplementedException();

    public E Define(out ILLabel label, string? labelName = null) => throw new NotImplementedException();

    public E Mark(ILLabel label) => throw new NotImplementedException();

    public E Mark(out ILLabel label, string? labelName = null) => throw new NotImplementedException();

    public E Return() => throw new NotImplementedException();

    public E Box<T>() => throw new NotImplementedException();

    public E Castclass<C>() where C : class => throw new NotImplementedException();

    public E Unbox<T>() => throw new NotImplementedException();

    public E BoxIfNeeded<T>() => throw new NotImplementedException();

    public E Conv<T>(bool overflowCheck = false, bool unsigned = false) => throw new NotImplementedException();

    public ITryCatchFinally<E> Try(Action<E, ILLabel> tryBlock) => throw new NotImplementedException();

    public E Leave(ILLabel label) => throw new NotImplementedException();

    public E Throw() => throw new NotImplementedException();

    public E ThrowNew<X>(params object?[] args) where X : Exception => throw new NotImplementedException();
}