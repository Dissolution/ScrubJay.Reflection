using ScrubJay.Reflection.Runtime.Emission.Instructions;

namespace ScrubJay.Reflection.Runtime.Emission;

internal class BackedBasicEmitter<TEmitter> : BasicEmitter<TEmitter>,
    IBasicEmitter<TEmitter>
    where TEmitter : IBasicEmitter<TEmitter>
{
    protected readonly ILGenerator _ilGenerator;
    protected readonly List<LocalBuilder> _locals = new(0);
    protected readonly List<Label> _labels = new(0);

    public BackedBasicEmitter(ILGenerator ilGenerator) : base()
    {
        _ilGenerator = ilGenerator;
    }

    protected override int Offset
    {
        get
        {
            return _ilGenerator.ILOffset;
        }
    }

    protected override EmitterLabel CreateEmitterLabel(string lblName)
    {
        Label label = _ilGenerator.DefineLabel();
        _labels.Add(label);
        EmitterLabel emitterLabel = new EmitterLabel(label, lblName);
        _emitterLabels.Add(emitterLabel);
        return emitterLabel;
    }

    protected override EmitterLocal CreateEmitterLocal(Type type, bool isPinned, string localName)
    {
        var local = _ilGenerator.DeclareLocal(type, isPinned);
        if (local.LocalIndex != _locals.Count)
            throw new InvalidOperationException();
        _locals.Add(local);
        EmitterLocal emitterLocal = new EmitterLocal(local, localName);
        _emitterLocals.Add(emitterLocal);
        return emitterLocal;
    }

    protected Label GetOrThrow(EmitterLabel emitterLabel)
    {
        base.ThrowIfInvalid(emitterLabel);
        return _labels[emitterLabel.Value];
    }

    protected LocalBuilder GetOrThrow(EmitterLocal emitterLocal)
    {
        base.ThrowIfInvalid(emitterLocal);
        return _locals[emitterLocal.LocalIndex];
    }

#region IGeneratorEmitter<TEmitter>
    public override TEmitter BeginExceptionBlock(out EmitterLabel label, string lblName = "")
    {
        var lbl = _ilGenerator.BeginExceptionBlock();
        label = new EmitterLabel(lbl, lblName);
        _emitterLabels.Add(label);
        AddInstruction(new ILGeneratorInstruction.BeginExceptionBlock(label));
        return _emitter;
    }

    public override TEmitter BeginCatchBlock(Type exceptionType)
    {
        _ilGenerator.BeginCatchBlock(exceptionType);
        return base.BeginCatchBlock(exceptionType);
    }

    public override TEmitter BeginFinallyBlock()
    {
        _ilGenerator.BeginFinallyBlock();
        return base.BeginFinallyBlock();
    }

    public override TEmitter BeginExceptFilterBlock()
    {
        _ilGenerator.BeginExceptFilterBlock();
        return base.BeginExceptFilterBlock();
    }

    public override TEmitter BeginFaultBlock()
    {
        _ilGenerator.BeginFaultBlock();
        return base.BeginFaultBlock();
    }

    public override TEmitter EndExceptionBlock()
    {
        _ilGenerator.EndExceptionBlock();
        return base.EndExceptionBlock();
    }

    public override TEmitter BeginScope()
    {
        _ilGenerator.BeginScope();
        return base.BeginScope();
    }

    public override TEmitter EndScope()
    {
        _ilGenerator.EndScope();
        return base.EndScope();
    }

    public override TEmitter UsingNamespace(string @namespace)
    {
        _ilGenerator.UsingNamespace(@namespace);
        return base.UsingNamespace(@namespace);
    }

    public override TEmitter MarkLabel(EmitterLabel label)
    {
        _ilGenerator.MarkLabel(label.Label);
        return base.MarkLabel(label);
    }

    public override TEmitter EmitCall(MethodInfo methodInfo, Type[]? optionalParameterTypes = null)
    {
        _ilGenerator.EmitCall(methodInfo.GetCallOpCode(), methodInfo, optionalParameterTypes);
        return base.EmitCall(methodInfo, optionalParameterTypes);
    }

    public override TEmitter EmitCalli(CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, Type[]? optionalParameterTypes = null)
    {
        _ilGenerator.EmitCalli(OpCodes.Calli, callingConvention, returnType, parameterTypes, optionalParameterTypes);
        return base.EmitCalli(callingConvention, returnType, parameterTypes, optionalParameterTypes);
    }

#if !NETSTANDARD2_0
    public override TEmitter EmitCalli(CallingConvention unmanagedCallConv, Type? returnType, Type[]? parameterTypes)
    {
        _ilGenerator.EmitCalli(OpCodes.Calli, unmanagedCallConv, returnType, parameterTypes);
        return base.EmitCalli(unmanagedCallConv, returnType, parameterTypes);
    }
#endif
#endregion
    #region Emit
    public override TEmitter Emit(OpCode opCode)
    {
        _ilGenerator.Emit(opCode);
        return base.Emit(opCode);
    }
    public override TEmitter Emit(OpCode opCode, byte uint8)
    {
        _ilGenerator.Emit(opCode, uint8);
        return base.Emit(opCode, uint8);
    }
    public override TEmitter Emit(OpCode opCode, sbyte int8)
    {
        _ilGenerator.Emit(opCode, int8);
        return base.Emit(opCode, int8);
    }
    public override TEmitter Emit(OpCode opCode, short int16)
    {
        _ilGenerator.Emit(opCode, int16);
        return base.Emit(opCode, int16);
    }
    public override TEmitter Emit(OpCode opCode, int int32)
    {
        _ilGenerator.Emit(opCode, int32);
        return base.Emit(opCode, int32);
    }
    public override TEmitter Emit(OpCode opCode, long int64)
    {
        _ilGenerator.Emit(opCode, int64);
        return base.Emit(opCode, int64);
    }
    public override TEmitter Emit(OpCode opCode, float float32)
    {
        _ilGenerator.Emit(opCode, float32);
        return base.Emit(opCode, float32);
    }
    public override TEmitter Emit(OpCode opCode, double float64)
    {
        _ilGenerator.Emit(opCode, float64);
        return base.Emit(opCode, float64);
    }
    public override TEmitter Emit(OpCode opCode, string str)
    {
        _ilGenerator.Emit(opCode, str);
        return base.Emit(opCode, str);
    }
    public override TEmitter Emit(OpCode opCode, EmitterLabel emitterLabel)
    {
        var label = GetOrThrow(emitterLabel);
        _ilGenerator.Emit(opCode, label);
        return base.Emit(opCode, emitterLabel);
    }
    public override TEmitter Emit(OpCode opCode, params EmitterLabel[] emitterLabels)
    {
        int count = emitterLabels.Length;
        var labels = new Label[count];
        for (var i = 0; i < count; i++)
        {
            labels[i] = GetOrThrow(emitterLabels[i]);
        }
        _ilGenerator.Emit(opCode, labels);
        return base.Emit(opCode, emitterLabels);
    }
    public override TEmitter Emit(OpCode opCode, EmitterLocal emitterLocal)
    {
        _ilGenerator.Emit(opCode, GetOrThrow(emitterLocal));
        return base.Emit(opCode, emitterLocal);
    }
    public override TEmitter Emit(OpCode opCode, FieldInfo field)
    {
        _ilGenerator.Emit(opCode, field);
        return base.Emit(opCode, field);
    }
    public override TEmitter Emit(OpCode opCode, ConstructorInfo ctor)
    {
        _ilGenerator.Emit(opCode, ctor);
        return base.Emit(opCode, ctor);
    }
    public override TEmitter Emit(OpCode opCode, MethodInfo method)
    {
        _ilGenerator.Emit(opCode, method);
        return base.Emit(opCode, method);
    }
    public override TEmitter Emit(OpCode opCode, Type type)
    {
        _ilGenerator.Emit(opCode, type);
        return base.Emit(opCode, type);
    }
    public override TEmitter Emit(OpCode opCode, SignatureHelper signature)
    {
        _ilGenerator.Emit(opCode, signature);
        return base.Emit(opCode, signature);
    }
#endregion
}