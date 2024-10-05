using ScrubJay.Reflection.Runtime.Emission.Instructions;

namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

public interface IILGeneratorEmitter<TEmitter> :
    IGeneratorEmitter<TEmitter>,
    IOpCodeEmitter<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    
}

internal class GeneratorEmitter<TEmitter> :
    IILGeneratorEmitter<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    protected readonly List<EmitterLabel> _emitterLabels = new(0);
    protected readonly List<EmitterLocal> _emitterLocals = new(0);
    protected readonly TEmitter _emitter;

    protected virtual int Offset { get; set; }

    public InstructionStream Instructions { get; } = new();
    
    public GeneratorEmitter()
    {
        _emitter = (TEmitter)(IILEmitter<TEmitter>)this;
    }

    protected virtual void AddInstruction(Instruction instruction)
    {
        var line = new InstructionLine(Offset, instruction);
        this.Instructions.Add(line);
        Offset += instruction.Size;
    }

    protected virtual EmitterLabel CreateEmitterLabel(string? labelName)
    {
        int index = _emitterLabels.Count;
        Label label = EmissionHelper.NewLabel(index);
        EmitterLabel emitterLabel = new EmitterLabel(label, labelName);
        _emitterLabels.Add(emitterLabel);
        return emitterLabel;
    }

    protected virtual void ThrowIfInvalid(EmitterLabel label)
    {
        int index = label.Value;
        if (index < 0 || index >= _emitterLabels.Count)
            throw new ArgumentException("", nameof(label));
        if (!_emitterLabels[index].Equals(label))
            throw new ArgumentException("", nameof(label));
    }


    protected virtual EmitterLocal CreateEmitterLocal(Type type, bool isPinned, string? localName)
    {
        int index = _emitterLocals.Count;
        EmitterLocal emitterLocal = new EmitterLocal(index, type, isPinned, localName);
        _emitterLocals.Add(emitterLocal);
        return emitterLocal;
    }

    protected virtual void ThrowIfInvalid(EmitterLocal local)
    {
        int index = local.LocalIndex;
        if (index < 0 || index >= _emitterLocals.Count)
            throw new ArgumentException("", nameof(local));
        if (!_emitterLocals[index].Equals(local))
            throw new ArgumentException("", nameof(local));
    }
    
    #region Generator
    public virtual TEmitter BeginExceptionBlock(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null)
    {
        label = CreateEmitterLabel(labelName);
        AddInstruction(new BeginExceptionBlockInstruction(label));
        return _emitter;
    }

    public virtual TEmitter BeginCatchBlock(Type exceptionType)
    {
        Validate.ThrowIfNull(exceptionType);
        if (!exceptionType.Implements<Exception>())
            throw new ArgumentException($"{exceptionType} is not a valid Exception Type", nameof(exceptionType));
        AddInstruction(new BeginCatchBlockInstruction(exceptionType));
        return _emitter;
    }
    public TEmitter BeginCatchBlock<TException>()
        where TException : Exception
    {
        return BeginCatchBlock(typeof(TException));
    }

    public virtual TEmitter BeginFinallyBlock()
    {
        AddInstruction(new BeginFinallyBlockInstruction());
        return _emitter;
    }

    public virtual TEmitter BeginExceptFilterBlock()
    {
        AddInstruction(new BeginExceptFilterBlockInstruction());
        return _emitter;
    }

    public virtual TEmitter BeginFaultBlock()
    {
        AddInstruction(new BeginFaultBlockInstruction());
        return _emitter;
    }

    public virtual TEmitter EndExceptionBlock()
    {
        AddInstruction(new EndExceptionBlockInstruction());
        return _emitter;
    }

    public virtual TEmitter BeginScope()
    {
        AddInstruction(new BeginScopeInstruction());
        return _emitter;
    }

    public virtual TEmitter EndScope()
    {
        AddInstruction(new EndScopeInstruction());
        return _emitter;
    }

    public virtual TEmitter UsingNamespace(string @namespace)
    {
        AddInstruction(new UsingNamespaceInstruction(@namespace));
        return _emitter;
    }

    public virtual TEmitter DeclareLocal(Type localType, out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null)
    {
        local = CreateEmitterLocal(localType, false, localName);
        AddInstruction(new DeclareLocalInstruction(local));
        return _emitter;
    }
    public TEmitter DeclareLocal<T>(out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null)
    {
        return DeclareLocal(typeof(T), out local, localName);
    }

    public virtual TEmitter DeclareLocal(Type localType, bool pinned, out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null)
    {
        local = CreateEmitterLocal(localType, pinned, localName);
        AddInstruction(new DeclareLocalInstruction(local));
        return _emitter;
    }
    public virtual TEmitter DeclareLocal<T>(bool pinned, out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null)
    {
        return DeclareLocal(typeof(T), pinned, out local, localName);
    }

    public virtual TEmitter DefineLabel(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null)
    {
        label = CreateEmitterLabel(labelName);
        AddInstruction(new DefineLabelInstruction(label));
        return _emitter;
    }

    public virtual TEmitter MarkLabel(EmitterLabel label)
    {
        ThrowIfInvalid(label);
        AddInstruction(new MarkLabelInstruction(label));
        return _emitter;
    }

    public virtual TEmitter EmitCall(MethodInfo methodInfo, Type[]? optionalParameterTypes = null)
    {
        AddInstruction(new CallVarargsInstruction(methodInfo, optionalParameterTypes));
        return _emitter;
    }

    public virtual TEmitter EmitCalli(CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, Type[]? optionalParameterTypes = null)
    {
        AddInstruction(new CallManagedInstruction(callingConvention, returnType, parameterTypes, optionalParameterTypes));
        return _emitter;
    }

    public virtual TEmitter EmitCalli(CallingConvention unmanagedCallConv, Type? returnType, Type[]? parameterTypes)
    {
        AddInstruction(new CallUnmanagedInstruction(unmanagedCallConv, returnType, parameterTypes));
        return _emitter;
    }
#endregion

#region OpCode
    public virtual TEmitter Emit(OpCode opCode)
    {
        if (opCode.OperandType != OperandType.InlineNone)
            throw new ArgumentException("", nameof(opCode));
        AddInstruction(new OpCodeInstruction(opCode));
        return _emitter;
    }

    public virtual TEmitter Emit(OpCode opCode, byte uint8)
    {
        AddInstruction(new OpCodeArgInstruction<byte>(opCode, uint8));
        return _emitter;
    }
    public virtual TEmitter Emit(OpCode opCode, sbyte int8)
    {
        AddInstruction(new OpCodeArgInstruction<sbyte>(opCode, int8));
        return _emitter;
    }
    public virtual TEmitter Emit(OpCode opCode, short int16)
    {
        AddInstruction(new OpCodeArgInstruction<short>(opCode, int16));
        return _emitter;
    }
    public virtual TEmitter Emit(OpCode opCode, int int32)
    {
        AddInstruction(new OpCodeArgInstruction<int>(opCode, int32));
        return _emitter;
    }
    public virtual TEmitter Emit(OpCode opCode, long int64)
    {
        AddInstruction(new OpCodeArgInstruction<long>(opCode, int64));
        return _emitter;
    }
    public virtual TEmitter Emit(OpCode opCode, float float32)
    {
        AddInstruction(new OpCodeArgInstruction<float>(opCode, float32));
        return _emitter;
    }
    public virtual TEmitter Emit(OpCode opCode, double float64)
    {
        AddInstruction(new OpCodeArgInstruction<double>(opCode, float64));
        return _emitter;
    }
    public virtual TEmitter Emit(OpCode opCode, string str)
    {
        AddInstruction(new OpCodeArgInstruction<string>(opCode, str));
        return _emitter;
    }
    public virtual TEmitter Emit(OpCode opCode, EmitterLabel label)
    {
        AddInstruction(new OpCodeArgInstruction<EmitterLabel>(opCode, label));
        return _emitter;
    }
    public virtual TEmitter Emit(OpCode opCode, params EmitterLabel[] labels)
    {
        AddInstruction(new OpCodeArgInstruction<EmitterLabel[]>(opCode, labels));
        return _emitter;
    }
    public virtual TEmitter Emit(OpCode opCode, EmitterLocal local)
    {
        AddInstruction(new OpCodeArgInstruction<EmitterLocal>(opCode, local));
        return _emitter;
    }
    public virtual TEmitter Emit(OpCode opCode, FieldInfo field)
    {
        AddInstruction(new OpCodeArgInstruction<FieldInfo>(opCode, field));
        return _emitter;
    }
    public virtual TEmitter Emit(OpCode opCode, ConstructorInfo ctor)
    {
        AddInstruction(new OpCodeArgInstruction<ConstructorInfo>(opCode, ctor));
        return _emitter;
    }
    public virtual TEmitter Emit(OpCode opCode, MethodInfo method)
    {
        AddInstruction(new OpCodeArgInstruction<MethodInfo>(opCode, method));
        return _emitter;
    }
    public virtual TEmitter Emit(OpCode opCode, Type type)
    {
        AddInstruction(new OpCodeArgInstruction<Type>(opCode, type));
        return _emitter;
    }
    public virtual TEmitter Emit(OpCode opCode, SignatureHelper signature)
    {
        AddInstruction(new OpCodeArgInstruction<SignatureHelper>(opCode, signature));
        return _emitter;
    }
#endregion

}