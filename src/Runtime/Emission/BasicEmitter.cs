using ScrubJay.Reflection.Runtime.Emission.Fluent;
using ScrubJay.Reflection.Runtime.Emission.Instructions;

namespace ScrubJay.Reflection.Runtime.Emission;

internal class BasicEmitter<TEmitter> :
    IGeneratorEmitter<TEmitter>,
    IOpCodeEmitter<TEmitter>,
    IOperationEmitter<TEmitter>,
    IBasicEmitter<TEmitter>,
    IILEmitter<TEmitter>,
    IILEmitter
    where TEmitter : IBasicEmitter<TEmitter>
{
    protected readonly List<EmitterLabel> _emitterLabels = new(0);
    protected readonly List<EmitterLocal> _emitterLocals = new(0);
    protected readonly TEmitter _emitter;

    protected virtual int Offset { get; set; }

    public InstructionStream Instructions { get; } = new();

    public IFluentEmitter Fluent => EmissionHelper.GetFluentEmitter((IBasicEmitter)_emitter);

    public BasicEmitter()
    {
        _emitter = (TEmitter)(IILEmitter<TEmitter>)this;
    }

    protected virtual void AddInstruction(Instruction instruction)
    {
        var line = new InstructionLine(Offset, instruction);
        this.Instructions.Add(line);
        Offset += instruction.Size;
    }

    protected virtual EmitterLabel CreateEmitterLabel(string lblName)
    {
        int index = _emitterLabels.Count;
        Label label = EmissionHelper.NewLabel(index);
        EmitterLabel emitterLabel = new EmitterLabel(label, lblName);
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


    protected virtual EmitterLocal CreateEmitterLocal(Type type, bool isPinned, string localName)
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

    public TEmitter Invoke(Action<TEmitter> emission)
    {
        Validation.Throw.IfNull(emission);
        emission(_emitter);
        return _emitter;
    }

    public TEmitter Invoke(Func<TEmitter, TEmitter> emission)
    {
        Validation.Throw.IfNull(emission);
        return emission(_emitter);
    }

#region Generator
    public virtual TEmitter BeginExceptionBlock(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string lblName = "")
    {
        label = CreateEmitterLabel(lblName);
        AddInstruction(new ILGeneratorInstruction.BeginExceptionBlock(label));
        return _emitter;
    }

    public virtual TEmitter BeginCatchBlock(Type exceptionType)
    {
        Validation.Throw.IfNull(exceptionType);
        if (!exceptionType.Implements<Exception>())
            throw new ArgumentException($"{exceptionType} is not a valid Exception Type", nameof(exceptionType));
        AddInstruction(new ILGeneratorInstruction.BeginCatchBlock(exceptionType));
        return _emitter;
    }
    public TEmitter BeginCatchBlock<TException>()
        where TException : Exception
    {
        return BeginCatchBlock(typeof(TException));
    }

    public virtual TEmitter BeginFinallyBlock()
    {
        AddInstruction(new ILGeneratorInstruction.BeginFinallyBlock());
        return _emitter;
    }

    public virtual TEmitter BeginExceptFilterBlock()
    {
        AddInstruction(new ILGeneratorInstruction.BeginExceptFilterBlock());
        return _emitter;
    }

    public virtual TEmitter BeginFaultBlock()
    {
        AddInstruction(new ILGeneratorInstruction.BeginFaultBlock());
        return _emitter;
    }

    public virtual TEmitter EndExceptionBlock()
    {
        AddInstruction(new ILGeneratorInstruction.EndExceptionBlock());
        return _emitter;
    }

    public virtual TEmitter BeginScope()
    {
        AddInstruction(new ILGeneratorInstruction.BeginScope());
        return _emitter;
    }

    public virtual TEmitter EndScope()
    {
        AddInstruction(new ILGeneratorInstruction.EndScope());
        return _emitter;
    }

    public virtual TEmitter UsingNamespace(string @namespace)
    {
        AddInstruction(new ILGeneratorInstruction.UsingNamespace(@namespace));
        return _emitter;
    }

    public virtual TEmitter DeclareLocal(Type localType, out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string localName = "")
    {
        local = CreateEmitterLocal(localType, false, localName);
        AddInstruction(new ILGeneratorInstruction.DeclareLocal(local));
        return _emitter;
    }
    public TEmitter DeclareLocal<T>(out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string localName = "")
    {
        return DeclareLocal(typeof(T), out local, localName);
    }

    public virtual TEmitter DeclareLocal(Type localType, bool pinned, out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string localName = "")
    {
        local = CreateEmitterLocal(localType, pinned, localName);
        AddInstruction(new ILGeneratorInstruction.DeclareLocal(local));
        return _emitter;
    }
    public virtual TEmitter DeclareLocal<T>(bool pinned, out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string localName = "")
    {
        return DeclareLocal(typeof(T), pinned, out local, localName);
    }

    public virtual TEmitter DefineLabel(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string labelName = "")
    {
        label = CreateEmitterLabel(labelName);
        AddInstruction(new ILGeneratorInstruction.DefineLabel(label));
        return _emitter;
    }

    public virtual TEmitter MarkLabel(EmitterLabel label)
    {
        ThrowIfInvalid(label);
        AddInstruction(new ILGeneratorInstruction.MarkLabel(label));
        return _emitter;
    }

    public virtual TEmitter EmitCall(MethodInfo methodInfo, Type[]? optionalParameterTypes = null)
    {
        AddInstruction(new ILGeneratorInstruction.CallVarargs(methodInfo, optionalParameterTypes));
        return _emitter;
    }

    public virtual TEmitter EmitCalli(CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, Type[]? optionalParameterTypes = null)
    {
        AddInstruction(new ILGeneratorInstruction.CallManaged(callingConvention, returnType, parameterTypes, optionalParameterTypes));
        return _emitter;
    }

    public virtual TEmitter EmitCalli(CallingConvention unmanagedCallConv, Type? returnType, Type[]? parameterTypes)
    {
        AddInstruction(new ILGeneratorInstruction.CallUnmanaged(unmanagedCallConv, returnType, parameterTypes));
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

#region Operation
    public TEmitter Endfilter() => Emit(OpCodes.Endfilter);
    public TEmitter Endfinally() => Emit(OpCodes.Endfinally);
    public TEmitter Arglist() => Emit(OpCodes.Arglist);
    public TEmitter Ldarg(int index) => Emit(OpCodes.Ldarg, index);
    public TEmitter Ldarg_S(int index) => Emit(OpCodes.Ldarg_S, index);
    public TEmitter Ldarg_0() => Emit(OpCodes.Ldarg_0);
    public TEmitter Ldarg_1() => Emit(OpCodes.Ldarg_1);
    public TEmitter Ldarg_2() => Emit(OpCodes.Ldarg_2);
    public TEmitter Ldarg_3() => Emit(OpCodes.Ldarg_3);
    public TEmitter Ldarga(int index) => Emit(OpCodes.Ldarga, index);
    public TEmitter Ldarga_S(int index) => Emit(OpCodes.Ldarga_S, index);
    public TEmitter Starg(int index) => Emit(OpCodes.Starg, index);
    public TEmitter Starg_S(int index) => Emit(OpCodes.Starg_S, index);
    public TEmitter Ldloc(EmitterLocal local) => Emit(OpCodes.Ldloc, local);
    public TEmitter Ldloc_S(EmitterLocal local) => Emit(OpCodes.Ldloc_S, local);
    public TEmitter Ldloc_0() => Emit(OpCodes.Ldloc_0);
    public TEmitter Ldloc_1() => Emit(OpCodes.Ldloc_1);
    public TEmitter Ldloc_2() => Emit(OpCodes.Ldloc_2);
    public TEmitter Ldloc_3() => Emit(OpCodes.Ldloc_3);
    public TEmitter Ldloca(EmitterLocal local) => Emit(OpCodes.Ldloca, local);
    public TEmitter Ldloca_S(EmitterLocal local) => Emit(OpCodes.Ldloca_S, local);
    public TEmitter Stloc(EmitterLocal local) => Emit(OpCodes.Stloc, local);
    public TEmitter Stloc_S(EmitterLocal local) => Emit(OpCodes.Stloc_S, local);
    public TEmitter Stloc_0() => Emit(OpCodes.Stloc_0);
    public TEmitter Stloc_1() => Emit(OpCodes.Stloc_1);
    public TEmitter Stloc_2() => Emit(OpCodes.Stloc_2);
    public TEmitter Stloc_3() => Emit(OpCodes.Stloc_3);
    public TEmitter Switch(params EmitterLabel[] labels) => Emit(OpCodes.Switch, labels);
    public TEmitter Call(MethodInfo method) => Emit(OpCodes.Call, method);
    public TEmitter Callvirt(MethodInfo method) => Emit(OpCodes.Callvirt, method);
    public TEmitter Constrained(Type type) => Emit(OpCodes.Constrained, type);
    public TEmitter Constrained<T>() => Constrained(typeof(T));
    public TEmitter Ldftn(MethodInfo method) => Emit(OpCodes.Ldftn, method);
    public TEmitter Ldvirtftn(MethodInfo method) => Emit(OpCodes.Ldvirtftn, method);
    public TEmitter Tailcall() => Emit(OpCodes.Tailcall);
    public TEmitter Break() => Emit(OpCodes.Break);
    public TEmitter Nop() => Emit(OpCodes.Nop);
    public TEmitter Prefix1() => Emit(OpCodes.Prefix1);
    public TEmitter Prefix2() => Emit(OpCodes.Prefix2);
    public TEmitter Prefix3() => Emit(OpCodes.Prefix3);
    public TEmitter Prefix4() => Emit(OpCodes.Prefix4);
    public TEmitter Prefix5() => Emit(OpCodes.Prefix5);
    public TEmitter Prefix6() => Emit(OpCodes.Prefix6);
    public TEmitter Prefix7() => Emit(OpCodes.Prefix7);
    public TEmitter Prefixref() => Emit(OpCodes.Prefixref);
    public TEmitter Ckfinite() => Emit(OpCodes.Ckfinite);
    public TEmitter Rethrow() => Emit(OpCodes.Rethrow);
    public TEmitter Throw() => Emit(OpCodes.Throw);
    public TEmitter Add() => Emit(OpCodes.Add);
    public TEmitter Add_Ovf() => Emit(OpCodes.Add_Ovf);
    public TEmitter Add_Ovf_Un() => Emit(OpCodes.Add_Ovf_Un);
    public TEmitter Div() => Emit(OpCodes.Div);
    public TEmitter Div_Un() => Emit(OpCodes.Div_Un);
    public TEmitter Mul() => Emit(OpCodes.Mul);
    public TEmitter Mul_Ovf() => Emit(OpCodes.Mul_Ovf);
    public TEmitter Mul_Ovf_Un() => Emit(OpCodes.Mul_Ovf_Un);
    public TEmitter Rem() => Emit(OpCodes.Rem);
    public TEmitter Rem_Un() => Emit(OpCodes.Rem_Un);
    public TEmitter Sub() => Emit(OpCodes.Sub);
    public TEmitter Sub_Ovf() => Emit(OpCodes.Sub_Ovf);
    public TEmitter Sub_Ovf_Un() => Emit(OpCodes.Sub_Ovf_Un);
    public TEmitter And() => Emit(OpCodes.And);
    public TEmitter Neg() => Emit(OpCodes.Neg);
    public TEmitter Not() => Emit(OpCodes.Not);
    public TEmitter Or() => Emit(OpCodes.Or);
    public TEmitter Shl() => Emit(OpCodes.Shl);
    public TEmitter Shr() => Emit(OpCodes.Shr);
    public TEmitter Shr_Un() => Emit(OpCodes.Shr_Un);
    public TEmitter Xor() => Emit(OpCodes.Xor);
    public TEmitter Br(EmitterLabel label) => Emit(OpCodes.Br, label);
    public TEmitter Br_S(EmitterLabel label) => Emit(OpCodes.Br_S, label);
    public TEmitter Jmp(MethodInfo method) => Emit(OpCodes.Jmp, method);
    public TEmitter Leave(EmitterLabel label) => Emit(OpCodes.Leave, label);
    public TEmitter Leave_S(EmitterLabel label) => Emit(OpCodes.Leave_S, label);
    public TEmitter Ret() => Emit(OpCodes.Ret);
    public TEmitter Brtrue(EmitterLabel label) => Emit(OpCodes.Brtrue, label);
    public TEmitter Brtrue_S(EmitterLabel label) => Emit(OpCodes.Brtrue_S, label);
    public TEmitter Brfalse(EmitterLabel label) => Emit(OpCodes.Brfalse, label);
    public TEmitter Brfalse_S(EmitterLabel label) => Emit(OpCodes.Brfalse_S, label);
    public TEmitter Beq(EmitterLabel label) => Emit(OpCodes.Beq, label);
    public TEmitter Beq_S(EmitterLabel label) => Emit(OpCodes.Beq_S, label);
    public TEmitter Bne_Un(EmitterLabel label) => Emit(OpCodes.Bne_Un, label);
    public TEmitter Bne_Un_S(EmitterLabel label) => Emit(OpCodes.Bne_Un_S, label);
    public TEmitter Bge(EmitterLabel label) => Emit(OpCodes.Bge, label);
    public TEmitter Bge_S(EmitterLabel label) => Emit(OpCodes.Bge_S, label);
    public TEmitter Bge_Un(EmitterLabel label) => Emit(OpCodes.Bge_Un, label);
    public TEmitter Bge_Un_S(EmitterLabel label) => Emit(OpCodes.Bge_Un_S, label);
    public TEmitter Bgt(EmitterLabel label) => Emit(OpCodes.Bgt, label);
    public TEmitter Bgt_S(EmitterLabel label) => Emit(OpCodes.Bgt_S, label);
    public TEmitter Bgt_Un(EmitterLabel label) => Emit(OpCodes.Bgt_Un, label);
    public TEmitter Bgt_Un_S(EmitterLabel label) => Emit(OpCodes.Bgt_Un_S, label);
    public TEmitter Ble(EmitterLabel label) => Emit(OpCodes.Ble, label);
    public TEmitter Ble_S(EmitterLabel label) => Emit(OpCodes.Ble_S, label);
    public TEmitter Ble_Un(EmitterLabel label) => Emit(OpCodes.Ble_Un, label);
    public TEmitter Ble_Un_S(EmitterLabel label) => Emit(OpCodes.Ble_Un_S, label);
    public TEmitter Blt(EmitterLabel label) => Emit(OpCodes.Blt, label);
    public TEmitter Blt_S(EmitterLabel label) => Emit(OpCodes.Blt_S, label);
    public TEmitter Blt_Un(EmitterLabel label) => Emit(OpCodes.Blt_Un, label);
    public TEmitter Blt_Un_S(EmitterLabel label) => Emit(OpCodes.Blt_Un_S, label);
    public TEmitter Box(Type type) => Emit(OpCodes.Box, type);
    public TEmitter Box<T>() => Box(typeof(T));
    public TEmitter Unbox(Type type) => Emit(OpCodes.Unbox, type);
    public TEmitter Unbox<T>() where T : struct => Unbox(typeof(T));
    public TEmitter Unbox_Any(Type type) => Emit(OpCodes.Unbox_Any, type);
    public TEmitter Unbox_Any<T>() => Unbox_Any(typeof(T));
    public TEmitter Castclass(Type type) => Emit(OpCodes.Castclass, type);
    public TEmitter Castclass<T>() where T : class => Castclass(typeof(T));
    public TEmitter Isinst(Type type) => Emit(OpCodes.Isinst, type);
    public TEmitter Isinst<T>() => Isinst(typeof(T));
    public TEmitter Conv_I() => Emit(OpCodes.Conv_I);
    public TEmitter Conv_Ovf_I() => Emit(OpCodes.Conv_Ovf_I);
    public TEmitter Conv_Ovf_I_Un() => Emit(OpCodes.Conv_Ovf_I_Un);
    public TEmitter Conv_I1() => Emit(OpCodes.Conv_I1);
    public TEmitter Conv_Ovf_I1() => Emit(OpCodes.Conv_Ovf_I1);
    public TEmitter Conv_Ovf_I1_Un() => Emit(OpCodes.Conv_Ovf_I1_Un);
    public TEmitter Conv_I2() => Emit(OpCodes.Conv_I2);
    public TEmitter Conv_Ovf_I2() => Emit(OpCodes.Conv_Ovf_I2);
    public TEmitter Conv_Ovf_I2_Un() => Emit(OpCodes.Conv_Ovf_I2_Un);
    public TEmitter Conv_I4() => Emit(OpCodes.Conv_I4);
    public TEmitter Conv_Ovf_I4() => Emit(OpCodes.Conv_Ovf_I4);
    public TEmitter Conv_Ovf_I4_Un() => Emit(OpCodes.Conv_Ovf_I4_Un);
    public TEmitter Conv_I8() => Emit(OpCodes.Conv_I8);
    public TEmitter Conv_Ovf_I8() => Emit(OpCodes.Conv_Ovf_I8);
    public TEmitter Conv_Ovf_I8_Un() => Emit(OpCodes.Conv_Ovf_I8_Un);
    public TEmitter Conv_U() => Emit(OpCodes.Conv_U);
    public TEmitter Conv_Ovf_U() => Emit(OpCodes.Conv_Ovf_U);
    public TEmitter Conv_Ovf_U_Un() => Emit(OpCodes.Conv_Ovf_U_Un);
    public TEmitter Conv_U1() => Emit(OpCodes.Conv_U1);
    public TEmitter Conv_Ovf_U1() => Emit(OpCodes.Conv_Ovf_U1);
    public TEmitter Conv_Ovf_U1_Un() => Emit(OpCodes.Conv_Ovf_U1_Un);
    public TEmitter Conv_U2() => Emit(OpCodes.Conv_U2);
    public TEmitter Conv_Ovf_U2() => Emit(OpCodes.Conv_Ovf_U2);
    public TEmitter Conv_Ovf_U2_Un() => Emit(OpCodes.Conv_Ovf_U2_Un);
    public TEmitter Conv_U4() => Emit(OpCodes.Conv_U4);
    public TEmitter Conv_Ovf_U4() => Emit(OpCodes.Conv_Ovf_U4);
    public TEmitter Conv_Ovf_U4_Un() => Emit(OpCodes.Conv_Ovf_U4_Un);
    public TEmitter Conv_U8() => Emit(OpCodes.Conv_U8);
    public TEmitter Conv_Ovf_U8() => Emit(OpCodes.Conv_Ovf_U8);
    public TEmitter Conv_Ovf_U8_Un() => Emit(OpCodes.Conv_Ovf_U8_Un);
    public TEmitter Conv_R_Un() => Emit(OpCodes.Conv_R_Un);
    public TEmitter Conv_R4() => Emit(OpCodes.Conv_R4);
    public TEmitter Conv_R8() => Emit(OpCodes.Conv_R8);
    public TEmitter Ceq() => Emit(OpCodes.Ceq);
    public TEmitter Cgt() => Emit(OpCodes.Cgt);
    public TEmitter Cgt_Un() => Emit(OpCodes.Cgt_Un);
    public TEmitter Clt() => Emit(OpCodes.Clt);
    public TEmitter Clt_Un() => Emit(OpCodes.Clt_Un);
    public TEmitter Cpblk() => Emit(OpCodes.Cpblk);
    public TEmitter Initblk() => Emit(OpCodes.Initblk);
    public TEmitter Localloc() => Emit(OpCodes.Localloc);
    public TEmitter Cpobj(Type type) => Emit(OpCodes.Cpobj, type);
    public TEmitter Cpobj<T>() where T : struct => Cpobj(typeof(T));
    public TEmitter Dup() => Emit(OpCodes.Dup);
    public TEmitter Initobj(Type type) => Emit(OpCodes.Initobj, type);
    public TEmitter Initobj<T>() where T : struct => Initobj(typeof(T));
    public TEmitter Newobj(ConstructorInfo ctor) => Emit(OpCodes.Newobj, ctor);
    public TEmitter Pop() => Emit(OpCodes.Pop);
    public TEmitter Ldc_I4(int value) => Emit(OpCodes.Ldc_I4, value);
    public TEmitter Ldc_I4_S(sbyte value) => Emit(OpCodes.Ldc_I4_S, value);
    public TEmitter Ldc_I4_M1() => Emit(OpCodes.Ldc_I4_M1);
    public TEmitter Ldc_I4_0() => Emit(OpCodes.Ldc_I4_0);
    public TEmitter Ldc_I4_1() => Emit(OpCodes.Ldc_I4_1);
    public TEmitter Ldc_I4_2() => Emit(OpCodes.Ldc_I4_2);
    public TEmitter Ldc_I4_3() => Emit(OpCodes.Ldc_I4_3);
    public TEmitter Ldc_I4_4() => Emit(OpCodes.Ldc_I4_4);
    public TEmitter Ldc_I4_5() => Emit(OpCodes.Ldc_I4_5);
    public TEmitter Ldc_I4_6() => Emit(OpCodes.Ldc_I4_6);
    public TEmitter Ldc_I4_7() => Emit(OpCodes.Ldc_I4_7);
    public TEmitter Ldc_I4_8() => Emit(OpCodes.Ldc_I4_8);
    public TEmitter Ldc_I8(long value) => Emit(OpCodes.Ldc_I8, value);
    public TEmitter Ldc_R4(float value) => Emit(OpCodes.Ldc_R4, value);
    public TEmitter Ldc_R8(double value) => Emit(OpCodes.Ldc_R8, value);
    public TEmitter Ldnull() => Emit(OpCodes.Ldnull);
    public TEmitter Ldstr(string str) => Emit(OpCodes.Ldstr, str);
    public TEmitter Ldtoken(Type type) => Emit(OpCodes.Ldtoken, type);
    public TEmitter Ldtoken(FieldInfo field) => Emit(OpCodes.Ldtoken, field);
    public TEmitter Ldtoken(MethodInfo method) => Emit(OpCodes.Ldtoken, method);
    public TEmitter Ldlen() => Emit(OpCodes.Ldlen);
    public TEmitter Ldelem(Type type) => Emit(OpCodes.Ldelem, type);
    public TEmitter Ldelem<T>() => Ldelem(typeof(T));
    public TEmitter Ldelem_I() => Emit(OpCodes.Ldelem_I);
    public TEmitter Ldelem_I1() => Emit(OpCodes.Ldelem_I1);
    public TEmitter Ldelem_I2() => Emit(OpCodes.Ldelem_I2);
    public TEmitter Ldelem_I4() => Emit(OpCodes.Ldelem_I4);
    public TEmitter Ldelem_I8() => Emit(OpCodes.Ldelem_I8);
    public TEmitter Ldelem_U1() => Emit(OpCodes.Ldelem_U1);
    public TEmitter Ldelem_U2() => Emit(OpCodes.Ldelem_U2);
    public TEmitter Ldelem_U4() => Emit(OpCodes.Ldelem_U4);
    public TEmitter Ldelem_R4() => Emit(OpCodes.Ldelem_R4);
    public TEmitter Ldelem_R8() => Emit(OpCodes.Ldelem_R8);
    public TEmitter Ldelem_Ref() => Emit(OpCodes.Ldelem_Ref);
    public TEmitter Ldelema(Type type) => Emit(OpCodes.Ldelema, type);
    public TEmitter Ldelema<T>() => Ldelema(typeof(T));
    public TEmitter Stelem(Type type) => Emit(OpCodes.Stelem, type);
    public TEmitter Stelem<T>() => Stelem(typeof(T));
    public TEmitter Stelem_I() => Emit(OpCodes.Stelem_I);
    public TEmitter Stelem_I1() => Emit(OpCodes.Stelem_I1);
    public TEmitter Stelem_I2() => Emit(OpCodes.Stelem_I2);
    public TEmitter Stelem_I4() => Emit(OpCodes.Stelem_I4);
    public TEmitter Stelem_I8() => Emit(OpCodes.Stelem_I8);
    public TEmitter Stelem_R4() => Emit(OpCodes.Stelem_R4);
    public TEmitter Stelem_R8() => Emit(OpCodes.Stelem_R8);
    public TEmitter Stelem_Ref() => Emit(OpCodes.Stelem_Ref);
    public TEmitter Newarr(Type type) => Emit(OpCodes.Newarr, type);
    public TEmitter Newarr<T>() => Newarr(typeof(T));
    public TEmitter Readonly() => Emit(OpCodes.Readonly);
    public TEmitter Ldfld(FieldInfo field) => Emit(OpCodes.Ldfld, field);
    public TEmitter Ldsfld(FieldInfo field) => Emit(OpCodes.Ldsfld, field);
    public TEmitter Ldflda(FieldInfo field) => Emit(OpCodes.Ldflda, field);
    public TEmitter Ldsflda(FieldInfo field) => Emit(OpCodes.Ldsflda, field);
    public TEmitter Stfld(FieldInfo field) => Emit(OpCodes.Stfld, field);
    public TEmitter Stsfld(FieldInfo field) => Emit(OpCodes.Stsfld, field);
    public TEmitter Ldobj(Type type) => Emit(OpCodes.Ldobj, type);
    public TEmitter Ldobj<T>() => Ldobj(typeof(T));
    public TEmitter Ldind_I() => Emit(OpCodes.Ldind_I);
    public TEmitter Ldind_I1() => Emit(OpCodes.Ldind_I1);
    public TEmitter Ldind_I2() => Emit(OpCodes.Ldind_I2);
    public TEmitter Ldind_I4() => Emit(OpCodes.Ldind_I4);
    public TEmitter Ldind_I8() => Emit(OpCodes.Ldind_I8);
    public TEmitter Ldind_U1() => Emit(OpCodes.Ldind_U1);
    public TEmitter Ldind_U2() => Emit(OpCodes.Ldind_U2);
    public TEmitter Ldind_U4() => Emit(OpCodes.Ldind_U4);
    public TEmitter Ldind_R4() => Emit(OpCodes.Ldind_R4);
    public TEmitter Ldind_R8() => Emit(OpCodes.Ldind_R8);
    public TEmitter Ldind_Ref() => Emit(OpCodes.Ldind_Ref);
    public TEmitter Stobj(Type type) => Emit(OpCodes.Stobj, type);
    public TEmitter Stobj<T>() => Stobj(typeof(T));
    public TEmitter Stind_I() => Emit(OpCodes.Stind_I);
    public TEmitter Stind_I1() => Emit(OpCodes.Stind_I1);
    public TEmitter Stind_I2() => Emit(OpCodes.Stind_I2);
    public TEmitter Stind_I4() => Emit(OpCodes.Stind_I4);
    public TEmitter Stind_I8() => Emit(OpCodes.Stind_I8);
    public TEmitter Stind_R4() => Emit(OpCodes.Stind_R4);
    public TEmitter Stind_R8() => Emit(OpCodes.Stind_R8);
    public TEmitter Stind_Ref() => Emit(OpCodes.Stind_Ref);
    public TEmitter Unaligned(int alignment) => Emit(OpCodes.Unaligned, alignment);
    public TEmitter Volatile() => Emit(OpCodes.Volatile);
    public TEmitter Mkrefany(Type type) => Emit(OpCodes.Mkrefany, type);
    public TEmitter Mkrefany<T>() => Mkrefany(typeof(T));
    public TEmitter Refanytype() => Emit(OpCodes.Refanytype);
    public TEmitter Refanyval(Type type) => Emit(OpCodes.Refanyval, type);
    public TEmitter Refanyval<T>() => Refanyval(typeof(T));
    public TEmitter Sizeof(Type type) => Emit(OpCodes.Sizeof, type);
    public TEmitter Sizeof<T>() where T : unmanaged => Sizeof(typeof(T));
#endregion
}