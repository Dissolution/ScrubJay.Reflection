using ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;
using ScrubJay.Reflection.Runtime.Emission.Instructions;
using ScrubJay.Text;
using ScrubJay.Validation;
// ReSharper disable IdentifierTypo
// ReSharper disable IdentifierTypo

namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

internal class Emitter
{
    public static ICleanEmitter GetCleanEmitter(ILGenerator? ilGenerator = null)
    {
        if (ilGenerator is not null)
        {
            ICleanEmitter<ICleanEmitter> emitter = new BackedEmitter<ICleanEmitter>(ilGenerator);
            return (ICleanEmitter)emitter;
        }
        else
        {
            ICleanEmitter<ICleanEmitter> emitter = new Emitter<ICleanEmitter>();
            return (ICleanEmitter)emitter;
        }
    }
}

internal partial class Emitter<TEmitter> : Emitter,
    ICleanEmitter<TEmitter>,
    IDirectEmitter<TEmitter>,
    ISimpleEmitter<TEmitter>
    where TEmitter : IILEmitter<TEmitter>// ICleanEmitter<TEmitter>, IDirectEmitter<TEmitter>, ISimpleEmitter<TEmitter>
{
    protected readonly List<EmitterLabel> _emitterLabels = new(0);
    protected readonly List<EmitterLocal> _emitterLocals = new(0);
    protected readonly TEmitter _emitter;

    protected virtual int Offset { get; set; }

    public InstructionStream Instructions { get; } = new();

    public ICleanEmitter CleanEmitter => (ICleanEmitter)_emitter;

    public IDirectEmitter DirectEmitter => (IDirectEmitter)_emitter;

    public ISimpleEmitter SimpleEmitter
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public Emitter()
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

    public TEmitter Invoke(Action<TEmitter> emission)
    {
        Validation.ThrowIf.Null(emission);
        emission(_emitter);
        return _emitter;
    }

    public TEmitter Invoke(Func<TEmitter, TEmitter> emission)
    {
        Validation.ThrowIf.Null(emission);
        return emission(_emitter);
    }

    public TEmitter LoadInstructions(InstructionStream instructions)
    {
        //InstructionLoader.LoadAll(_emitter, instructions);
        throw new NotImplementedException();
        return _emitter;
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
        Validation.ThrowIf.Null(exceptionType);
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

#region Operation
    public TEmitter Endfilter() => Emit(OpCodes.Endfilter);
    public TEmitter Endfinally() => Emit(OpCodes.Endfinally);
    public TEmitter Arglist() => Emit(OpCodes.Arglist);
    public TEmitter Switch(params EmitterLabel[] labels) => Emit(OpCodes.Switch, labels);
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
    public TEmitter And() => Emit(OpCodes.And);
    public TEmitter Neg() => Emit(OpCodes.Neg);
    public TEmitter Not() => Emit(OpCodes.Not);
    public TEmitter Or() => Emit(OpCodes.Or);
    public TEmitter Shl() => Emit(OpCodes.Shl);
    public TEmitter Xor() => Emit(OpCodes.Xor);
    public TEmitter Jmp(MethodInfo method) => Emit(OpCodes.Jmp, method);
    public TEmitter Ret() => Emit(OpCodes.Ret);
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
    public TEmitter Ceq() => Emit(OpCodes.Ceq);
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
    public TEmitter Ldnull() => Emit(OpCodes.Ldnull);
    public TEmitter Ldstr(string str) => Emit(OpCodes.Ldstr, str);
    public TEmitter Ldtoken(Type type) => Emit(OpCodes.Ldtoken, type);
    public TEmitter Ldtoken(FieldInfo field) => Emit(OpCodes.Ldtoken, field);
    public TEmitter Ldtoken(MethodInfo method) => Emit(OpCodes.Ldtoken, method);
    public TEmitter Ldlen() => Emit(OpCodes.Ldlen);
    public TEmitter Newarr(Type type) => Emit(OpCodes.Newarr, type);
    public TEmitter Newarr<T>() => Newarr(typeof(T));
    public TEmitter Readonly() => Emit(OpCodes.Readonly);
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

#region Clean
    public TEmitter Ldarg(int index)
    {
        ThrowIf.NotBetween(index, 0, short.MaxValue);
        return index switch
        {
            0 => Emit(OpCodes.Ldarg_0),
            1 => Emit(OpCodes.Ldarg_1),
            2 => Emit(OpCodes.Ldarg_2),
            3 => Emit(OpCodes.Ldarg_3),
            <= byte.MaxValue => Emit(OpCodes.Ldarg_S, (byte)index),
            _ => Emit(OpCodes.Ldarg, (short)index),
        };
    }
    public TEmitter Ldarga(int index)
    {
        ThrowIf.NotBetween(index, 0, short.MaxValue);
        if (index <= byte.MaxValue)
            return Emit(OpCodes.Ldarga_S, (byte)index);
        return Emit(OpCodes.Ldarga, (short)index);
    }
    public TEmitter Starg(int index)
    {
        ThrowIf.NotBetween(index, 0, short.MaxValue);
        if (index <= byte.MaxValue)
            return Emit(OpCodes.Starg_S, (byte)index);
        return Emit(OpCodes.Starg, (short)index);
    }
    public TEmitter Ldloc(int index)
    {
        ThrowIf.NotBetween(index, 0, short.MaxValue);
        return index switch
        {
            0 => Emit(OpCodes.Ldloc_0),
            1 => Emit(OpCodes.Ldloc_1),
            2 => Emit(OpCodes.Ldloc_2),
            3 => Emit(OpCodes.Ldloc_3),
            <= byte.MaxValue => Emit(OpCodes.Ldloc_S, (byte)index),
            _ => Emit(OpCodes.Ldloc, (short)index),
        };
    }
    public TEmitter Ldloc(EmitterLocal local)
    {
        int index = local.LocalIndex;
        ThrowIf.NotBetween(index, 0, short.MaxValue, nameof(local));
        return index switch
        {
            0 => Emit(OpCodes.Ldloc_0),
            1 => Emit(OpCodes.Ldloc_1),
            2 => Emit(OpCodes.Ldloc_2),
            3 => Emit(OpCodes.Ldloc_3),
            <= byte.MaxValue => Emit(OpCodes.Ldloc_S, local),
            _ => Emit(OpCodes.Ldloc, local),
        };
    }
    public TEmitter Ldloca(int index)
    {
        ThrowIf.NotBetween(index, 0, short.MaxValue);
        return index switch
        {
            <= byte.MaxValue => Emit(OpCodes.Ldloca_S, (byte)index),
            _ => Emit(OpCodes.Ldloca, (short)index),
        };
    }
    public TEmitter Ldloca(EmitterLocal local)
    {
        int index = local.LocalIndex;
        ThrowIf.NotBetween(index, 0, short.MaxValue, nameof(local));
        return index switch
        {
            <= byte.MaxValue => Emit(OpCodes.Ldloca_S, local),
            _ => Emit(OpCodes.Ldloca, local),
        };
    }
    public TEmitter Stloc(int index)
    {
        ThrowIf.NotBetween(index, 0, short.MaxValue);
        return index switch
        {
            0 => Emit(OpCodes.Stloc_0),
            1 => Emit(OpCodes.Stloc_1),
            2 => Emit(OpCodes.Stloc_2),
            3 => Emit(OpCodes.Stloc_3),
            <= byte.MaxValue => Emit(OpCodes.Stloc_S, (byte)index),
            _ => Emit(OpCodes.Stloc, (short)index),
        };
    }
    public TEmitter Stloc(EmitterLocal local)
    {
        int index = local.LocalIndex;
        ThrowIf.NotBetween(index, 0, short.MaxValue, nameof(local));
        return index switch
        {
            0 => Emit(OpCodes.Stloc_0),
            1 => Emit(OpCodes.Stloc_1),
            2 => Emit(OpCodes.Stloc_2),
            3 => Emit(OpCodes.Stloc_3),
            <= byte.MaxValue => Emit(OpCodes.Stloc_S, (byte)index),
            _ => Emit(OpCodes.Stloc, (short)index),
        };
    }
    public TEmitter Call(MethodBase method)
    {
        if (method is ConstructorInfo ctor)
        {
            return Emit(OpCodes.Newobj, ctor);
        }
        else if (method is MethodInfo info)
        {
            return Emit(info.GetCallOpCode(), info);
        }
        else
        {
            throw new ArgumentException(null, nameof(method));
        }
    }
    public TEmitter Add(bool unsigned = false, bool checkOverflow = false)
    {
        return Emit(checkOverflow ? unsigned ? OpCodes.Add_Ovf_Un : OpCodes.Add_Ovf : OpCodes.Add);
    }
    public TEmitter Div(bool unsigned = false)
    {
        return Emit(unsigned ? OpCodes.Div_Un : OpCodes.Div);
    }
    public TEmitter Mul(bool unsigned = false, bool checkOverflow = false)
    {
        return Emit(checkOverflow ? unsigned ? OpCodes.Mul_Ovf_Un : OpCodes.Mul_Ovf : OpCodes.Mul);
    }
    public TEmitter Rem(bool unsigned = false)
    {
        return Emit(unsigned ? OpCodes.Rem_Un : OpCodes.Rem);
    }
    public TEmitter Sub(bool unsigned = false, bool checkOverflow = false)
    {
        return Emit(checkOverflow ? unsigned ? OpCodes.Sub_Ovf_Un : OpCodes.Sub_Ovf : OpCodes.Sub);
    }


    public TEmitter Shr(bool unsigned = false)
    {
        return Emit(unsigned ? OpCodes.Shr_Un : OpCodes.Shr);
    }


    public TEmitter Br(EmitterLabel label)
    {
        return Emit(label.IsShortForm ? OpCodes.Br_S : OpCodes.Br, label);
    }
    public TEmitter Leave(EmitterLabel label)
    {
        return Emit(label.IsShortForm ? OpCodes.Leave_S : OpCodes.Leave, label);
    }
    public TEmitter Brtrue(EmitterLabel label)
    {
        return Emit(label.IsShortForm ? OpCodes.Brtrue_S : OpCodes.Brtrue, label);
    }
    public TEmitter Brfalse(EmitterLabel label)
    {
        return Emit(label.IsShortForm ? OpCodes.Brfalse_S : OpCodes.Brfalse, label);
    }
    public TEmitter Beq(EmitterLabel label)
    {
        return Emit(label.IsShortForm ? OpCodes.Beq_S : OpCodes.Beq, label);
    }
    public TEmitter Bne(EmitterLabel label)
    {
        return Emit(label.IsShortForm ? OpCodes.Bne_Un_S : OpCodes.Bne_Un, label);
    }
    public TEmitter Bge(EmitterLabel label, bool unsigned = false)
    {
        OpCode opCode;
        if (unsigned)
            if (label.IsShortForm)
                opCode = OpCodes.Bge_Un_S;
            else
                opCode = OpCodes.Bge_Un;
        else if (label.IsShortForm)
            opCode = OpCodes.Bge_S;
        else
            opCode = OpCodes.Bge;
        return Emit(opCode, label);
    }
    public TEmitter Bgt(EmitterLabel label, bool unsigned = false)
    {
        OpCode opCode;
        if (unsigned)
            if (label.IsShortForm)
                opCode = OpCodes.Bgt_Un_S;
            else
                opCode = OpCodes.Bgt_Un;
        else if (label.IsShortForm)
            opCode = OpCodes.Bgt_S;
        else
            opCode = OpCodes.Bgt;
        return Emit(opCode, label);
    }
    public TEmitter Ble(EmitterLabel label, bool unsigned = false)
    {
        OpCode opCode;
        if (unsigned)
            if (label.IsShortForm)
                opCode = OpCodes.Ble_Un_S;
            else
                opCode = OpCodes.Ble_Un;
        else if (label.IsShortForm)
            opCode = OpCodes.Ble_S;
        else
            opCode = OpCodes.Ble;
        return Emit(opCode, label);
    }
    public TEmitter Blt(EmitterLabel label, bool unsigned = false)
    {
        OpCode opCode;
        if (unsigned)
            if (label.IsShortForm)
                opCode = OpCodes.Blt_Un_S;
            else
                opCode = OpCodes.Blt_Un;
        else if (label.IsShortForm)
            opCode = OpCodes.Blt_S;
        else
            opCode = OpCodes.Blt;
        return Emit(opCode, label);
    }


    public TEmitter Conv(Type type, bool unsigned = false, bool checkOverflow = false)
    {
        // Conv_I
        if (type == typeof(IntPtr) || type == typeof(nint))
        {
            if (checkOverflow)
            {
                if (unsigned)
                {
                    return Emit(OpCodes.Conv_Ovf_I_Un);
                }
                return Emit(OpCodes.Conv_Ovf_I);
            }
            return Emit(OpCodes.Conv_I);
        }
        // Conv_I1
        if (type == typeof(sbyte))
        {
            if (checkOverflow)
            {
                if (unsigned)
                {
                    return Emit(OpCodes.Conv_Ovf_I1_Un);
                }
                return Emit(OpCodes.Conv_Ovf_I1);
            }
            return Emit(OpCodes.Conv_I1);
        }
        // Conv_I2
        if (type == typeof(short))
        {
            if (checkOverflow)
            {
                if (unsigned)
                {
                    return Emit(OpCodes.Conv_Ovf_I2_Un);
                }
                return Emit(OpCodes.Conv_Ovf_I2);
            }
            return Emit(OpCodes.Conv_I2);
        }
        // Conv_I4
        if (type == typeof(int))
        {
            if (checkOverflow)
            {
                if (unsigned)
                {
                    return Emit(OpCodes.Conv_Ovf_I4_Un);
                }
                return Emit(OpCodes.Conv_Ovf_I4);
            }
            return Emit(OpCodes.Conv_I4);
        }
        // Conv_I8
        if (type == typeof(long))
        {
            if (checkOverflow)
            {
                if (unsigned)
                {
                    return Emit(OpCodes.Conv_Ovf_I8_Un);
                }
                return Emit(OpCodes.Conv_Ovf_I8);
            }
            return Emit(OpCodes.Conv_I8);
        }
        // Conv_U
        if (type == typeof(UIntPtr) || type == typeof(nuint))
        {
            if (checkOverflow)
            {
                if (unsigned)
                {
                    return Emit(OpCodes.Conv_Ovf_U_Un);
                }
                return Emit(OpCodes.Conv_Ovf_U);
            }
            return Emit(OpCodes.Conv_U);
        }
        // Conv_U1
        if (type == typeof(byte))
        {
            if (checkOverflow)
            {
                if (unsigned)
                {
                    return Emit(OpCodes.Conv_Ovf_U1_Un);
                }
                return Emit(OpCodes.Conv_Ovf_U1);
            }
            return Emit(OpCodes.Conv_U1);
        }
        // Conv_U2
        if (type == typeof(ushort))
        {
            if (checkOverflow)
            {
                if (unsigned)
                {
                    return Emit(OpCodes.Conv_Ovf_U2_Un);
                }
                return Emit(OpCodes.Conv_Ovf_U2);
            }
            return Emit(OpCodes.Conv_U2);
        }
        // Conv_U4
        if (type == typeof(uint))
        {
            if (checkOverflow)
            {
                if (unsigned)
                {
                    return Emit(OpCodes.Conv_Ovf_U4_Un);
                }
                return Emit(OpCodes.Conv_Ovf_U4);
            }
            return Emit(OpCodes.Conv_U4);
        }
        // Conv_U8
        if (type == typeof(ulong))
        {
            if (checkOverflow)
            {
                if (unsigned)
                {
                    return Emit(OpCodes.Conv_Ovf_U8_Un);
                }
                return Emit(OpCodes.Conv_Ovf_U8);
            }
            return Emit(OpCodes.Conv_U8);
        }
        // Conv_R(4)
        if (type == typeof(float))
        {
            if (unsigned)
                return Emit(OpCodes.Conv_R_Un);
            return Emit(OpCodes.Conv_R8);
        }
        // Conv_R8
        if (type == typeof(double))
        {
            return Emit(OpCodes.Conv_R8);
        }
        throw new ArgumentException();
    }
    public TEmitter Conv<T>(bool unsigned = false, bool checkOverflow = false)
        => Conv(typeof(T), unsigned, checkOverflow);

    public TEmitter Cgt(bool unsigned = false)
    {
        return Emit(unsigned ? OpCodes.Cgt_Un : OpCodes.Cgt);
    }
    public TEmitter Clt(bool unsigned = false)
    {
        return Emit(unsigned ? OpCodes.Clt_Un : OpCodes.Clt);
    }

    public TEmitter Ldc(int value)
    {
        return value switch
        {
            -1 => Emit(OpCodes.Ldc_I4_M1),
            0 => Emit(OpCodes.Ldc_I4_0),
            1 => Emit(OpCodes.Ldc_I4_1),
            2 => Emit(OpCodes.Ldc_I4_2),
            3 => Emit(OpCodes.Ldc_I4_3),
            4 => Emit(OpCodes.Ldc_I4_4),
            5 => Emit(OpCodes.Ldc_I4_5),
            6 => Emit(OpCodes.Ldc_I4_6),
            7 => Emit(OpCodes.Ldc_I4_7),
            8 => Emit(OpCodes.Ldc_I4_8),
            >= sbyte.MinValue and <= sbyte.MaxValue => Emit(OpCodes.Ldc_I4_S, (sbyte)value),
            _ => Emit(OpCodes.Ldc_I4, value),
        };
    }
    public TEmitter Ldc(long value)
    {
        return Emit(OpCodes.Ldc_I8, value);
    }
    public TEmitter Ldc(float value)
    {
        return Emit(OpCodes.Ldc_R4, value);
    }
    public TEmitter Ldc(double value)
    {
        return Emit(OpCodes.Ldc_R8, value);
    }

    public TEmitter Ldelem(Type type)
    {
        if (type == typeof(IntPtr) || type == typeof(nint))
        {
            return Emit(OpCodes.Ldelem_I);
        }
        if (type == typeof(sbyte))
        {
            return Emit(OpCodes.Ldelem_I1);
        }
        if (type == typeof(short))
        {
            return Emit(OpCodes.Ldelem_I2);
        }
        if (type == typeof(int))
        {
            return Emit(OpCodes.Ldelem_I4);
        }
        if (type == typeof(long))
        {
            return Emit(OpCodes.Ldelem_I8);
        }
        if (type == typeof(byte))
        {
            return Emit(OpCodes.Ldelem_U1);
        }
        if (type == typeof(ushort))
        {
            return Emit(OpCodes.Ldelem_U2);
        }
        if (type == typeof(uint))
        {
            return Emit(OpCodes.Ldelem_U4);
        }
        if (type == typeof(float))
        {
            return Emit(OpCodes.Ldelem_R4);
        }
        if (type == typeof(double))
        {
            return Emit(OpCodes.Ldelem_R8);
        }
        if (type == typeof(object))
        {
            return Emit(OpCodes.Ldelem_Ref);
        }
        return Emit(OpCodes.Ldelem, type);
    }
    public TEmitter Ldelem<T>() => Ldelem(typeof(T));
    public TEmitter Ldelema(Type type) => Emit(OpCodes.Ldelema, type);
    public TEmitter Ldelema<T>() => Ldelema(typeof(T));
    public TEmitter Stelem(Type type)
    {
        if (type == typeof(IntPtr) || type == typeof(nint))
        {
            return Emit(OpCodes.Stelem_I);
        }
        if (type == typeof(sbyte))
        {
            return Emit(OpCodes.Stelem_I1);
        }
        if (type == typeof(short))
        {
            return Emit(OpCodes.Stelem_I2);
        }
        if (type == typeof(int))
        {
            return Emit(OpCodes.Stelem_I4);
        }
        if (type == typeof(long))
        {
            return Emit(OpCodes.Stelem_I8);
        }
        if (type == typeof(float))
        {
            return Emit(OpCodes.Stelem_R4);
        }
        if (type == typeof(double))
        {
            return Emit(OpCodes.Stelem_R8);
        }
        if (type == typeof(object))
        {
            return Emit(OpCodes.Stelem_Ref);
        }
        return Emit(OpCodes.Stelem, type);
    }
    public TEmitter Stelem<T>() => Stelem(typeof(T));

    public TEmitter Ldfld(FieldInfo field)
    {
        return Emit(field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field);
    }
    public TEmitter Ldflda(FieldInfo field)
    {
        return Emit(field.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda, field);
    }
    public TEmitter Stfld(FieldInfo field)
    {
        return Emit(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field);
    }


    public TEmitter Ldind(Type type)
    {
        if (type == typeof(IntPtr) || type == typeof(nint))
        {
            return Emit(OpCodes.Ldind_I);
        }
        if (type == typeof(sbyte))
        {
            return Emit(OpCodes.Ldind_I1);
        }
        if (type == typeof(short))
        {
            return Emit(OpCodes.Ldind_I2);
        }
        if (type == typeof(int))
        {
            return Emit(OpCodes.Ldind_I4);
        }
        if (type == typeof(long))
        {
            return Emit(OpCodes.Ldind_I8);
        }
        if (type == typeof(byte))
        {
            return Emit(OpCodes.Ldind_U1);
        }
        if (type == typeof(ushort))
        {
            return Emit(OpCodes.Ldind_U2);
        }
        if (type == typeof(uint))
        {
            return Emit(OpCodes.Ldind_U4);
        }
        if (type == typeof(float))
        {
            return Emit(OpCodes.Ldind_R4);
        }
        if (type == typeof(double))
        {
            return Emit(OpCodes.Ldind_R8);
        }
        if (type == typeof(object))
        {
            return Emit(OpCodes.Ldind_Ref);
        }
        return Emit(OpCodes.Ldobj, type);
    }
    public TEmitter Ldind<T>() => Ldind(typeof(T));

    public TEmitter Stind(Type type)
    {
        if (type == typeof(IntPtr) || type == typeof(nint))
        {
            return Emit(OpCodes.Stind_I);
        }
        if (type == typeof(sbyte))
        {
            return Emit(OpCodes.Stind_I1);
        }
        if (type == typeof(short))
        {
            return Emit(OpCodes.Stind_I2);
        }
        if (type == typeof(int))
        {
            return Emit(OpCodes.Stind_I4);
        }
        if (type == typeof(long))
        {
            return Emit(OpCodes.Stind_I8);
        }
        if (type == typeof(float))
        {
            return Emit(OpCodes.Stind_R4);
        }
        if (type == typeof(double))
        {
            return Emit(OpCodes.Stind_R8);
        }
        if (type == typeof(object))
        {
            return Emit(OpCodes.Stind_Ref);
        }
        return Emit(OpCodes.Ldobj, type);
    }
    public TEmitter Stind<T>() => Stind(typeof(T));
#endregion

#region Direct
    public TEmitter Ldarg(short index) => Emit(OpCodes.Ldarg, index);
    public TEmitter Ldarg_S(byte index) => Emit(OpCodes.Ldarg_S, index);
    public TEmitter Ldarg_0() => Emit(OpCodes.Ldarg_0);
    public TEmitter Ldarg_1() => Emit(OpCodes.Ldarg_1);
    public TEmitter Ldarg_2() => Emit(OpCodes.Ldarg_2);
    public TEmitter Ldarg_3() => Emit(OpCodes.Ldarg_3);
    public TEmitter Ldarga(short index) => Emit(OpCodes.Ldarga, index);
    public TEmitter Ldarga_S(byte index) => Emit(OpCodes.Ldarga_S, index);
    public TEmitter Starg(short index) => Emit(OpCodes.Starg, index);
    public TEmitter Starg_S(byte index) => Emit(OpCodes.Starg_S, index);
    public TEmitter Ldloc(short index) => Emit(OpCodes.Ldloc, index);
    TEmitter IDirectOperationEmitter<TEmitter>.Ldloc(EmitterLocal local) => Emit(OpCodes.Ldloc, local);
    public TEmitter Ldloc_S(byte index) => Emit(OpCodes.Ldloc_S, index);
    public TEmitter Ldloc_S(EmitterLocal local) => Emit(OpCodes.Ldloc_S, local);
    public TEmitter Ldloc_0() => Emit(OpCodes.Ldloc_0);
    public TEmitter Ldloc_1() => Emit(OpCodes.Ldloc_1);
    public TEmitter Ldloc_2() => Emit(OpCodes.Ldloc_2);
    public TEmitter Ldloc_3() => Emit(OpCodes.Ldloc_3);
    public TEmitter Ldloca(short index) => Emit(OpCodes.Ldloca, index);
    public TEmitter Ldloca_S(byte index) => Emit(OpCodes.Ldloca_S, index);
    TEmitter IDirectOperationEmitter<TEmitter>.Ldloca(EmitterLocal local) => Emit(OpCodes.Ldloca, local);
    public TEmitter Ldloca_S(EmitterLocal local) => Emit(OpCodes.Ldloca_S, local);
    public TEmitter Stloc(short index) => Emit(OpCodes.Stloc, index);
    public TEmitter Stloc_S(byte index) => Emit(OpCodes.Stloc_S, index);
    TEmitter IDirectOperationEmitter<TEmitter>.Stloc(EmitterLocal local) => Emit(OpCodes.Stloc, local);
    public TEmitter Stloc_S(EmitterLocal local) => Emit(OpCodes.Stloc_S, local);
    public TEmitter Stloc_0() => Emit(OpCodes.Stloc_0);
    public TEmitter Stloc_1() => Emit(OpCodes.Stloc_1);
    public TEmitter Stloc_2() => Emit(OpCodes.Stloc_2);
    public TEmitter Stloc_3() => Emit(OpCodes.Stloc_3);
    TEmitter IDirectOperationEmitter<TEmitter>.Call(MethodInfo method) => Emit(OpCodes.Call, method);
    public TEmitter Callvirt(MethodInfo method) => Emit(OpCodes.Callvirt, method);
    TEmitter IDirectOperationEmitter<TEmitter>.Add() => Emit(OpCodes.Add);
    public TEmitter Add_Ovf() => Emit(OpCodes.Add_Ovf);
    public TEmitter Add_Ovf_Un() => Emit(OpCodes.Add_Ovf_Un);
    TEmitter IDirectOperationEmitter<TEmitter>.Div() => Emit(OpCodes.Div);
    public TEmitter Div_Un() => Emit(OpCodes.Div_Un);
    TEmitter IDirectOperationEmitter<TEmitter>.Mul() => Emit(OpCodes.Mul);
    public TEmitter Mul_Ovf() => Emit(OpCodes.Mul_Ovf);
    public TEmitter Mul_Ovf_Un() => Emit(OpCodes.Mul_Ovf_Un);
    TEmitter IDirectOperationEmitter<TEmitter>.Rem() => Emit(OpCodes.Rem);
    public TEmitter Rem_Un() => Emit(OpCodes.Rem_Un);
    TEmitter IDirectOperationEmitter<TEmitter>.Sub() => Emit(OpCodes.Sub);
    public TEmitter Sub_Ovf() => Emit(OpCodes.Sub_Ovf);
    public TEmitter Sub_Ovf_Un() => Emit(OpCodes.Sub_Ovf_Un);
    TEmitter IDirectOperationEmitter<TEmitter>.Shr() => Emit(OpCodes.Shr);
    public TEmitter Shr_Un() => Emit(OpCodes.Shr_Un);
    TEmitter IDirectOperationEmitter<TEmitter>.Br(EmitterLabel label) => Emit(OpCodes.Br, label);
    public TEmitter Br_S(EmitterLabel label) => Emit(OpCodes.Br_S, label);
    TEmitter IDirectOperationEmitter<TEmitter>.Leave(EmitterLabel label) => Emit(OpCodes.Leave, label);
    public TEmitter Leave_S(EmitterLabel label) => Emit(OpCodes.Leave_S, label);
    TEmitter IDirectOperationEmitter<TEmitter>.Brtrue(EmitterLabel label) => Emit(OpCodes.Brtrue, label);
    public TEmitter Brtrue_S(EmitterLabel label) => Emit(OpCodes.Brtrue_S, label);
    TEmitter IDirectOperationEmitter<TEmitter>.Brfalse(EmitterLabel label) => Emit(OpCodes.Brfalse, label);
    public TEmitter Brfalse_S(EmitterLabel label) => Emit(OpCodes.Brfalse_S, label);
    TEmitter IDirectOperationEmitter<TEmitter>.Beq(EmitterLabel label) => Emit(OpCodes.Beq, label);
    public TEmitter Beq_S(EmitterLabel label) => Emit(OpCodes.Beq_S, label);
    public TEmitter Bne_Un(EmitterLabel label) => Emit(OpCodes.Bne_Un, label);
    public TEmitter Bne_Un_S(EmitterLabel label) => Emit(OpCodes.Bne_Un_S, label);
    TEmitter IDirectOperationEmitter<TEmitter>.Bge(EmitterLabel label) => Emit(OpCodes.Bge, label);
    public TEmitter Bge_S(EmitterLabel label) => Emit(OpCodes.Bge_S, label);
    public TEmitter Bge_Un(EmitterLabel label) => Emit(OpCodes.Bge_Un, label);
    public TEmitter Bge_Un_S(EmitterLabel label) => Emit(OpCodes.Bge_Un_S, label);
    TEmitter IDirectOperationEmitter<TEmitter>.Bgt(EmitterLabel label) => Emit(OpCodes.Bgt, label);
    public TEmitter Bgt_S(EmitterLabel label) => Emit(OpCodes.Bgt_S, label);
    public TEmitter Bgt_Un(EmitterLabel label) => Emit(OpCodes.Bgt_Un, label);
    public TEmitter Bgt_Un_S(EmitterLabel label) => Emit(OpCodes.Bgt_Un_S, label);
    TEmitter IDirectOperationEmitter<TEmitter>.Ble(EmitterLabel label) => Emit(OpCodes.Ble, label);
    public TEmitter Ble_S(EmitterLabel label) => Emit(OpCodes.Ble_S, label);
    public TEmitter Ble_Un(EmitterLabel label) => Emit(OpCodes.Ble_Un, label);
    public TEmitter Ble_Un_S(EmitterLabel label) => Emit(OpCodes.Ble_Un_S, label);
    TEmitter IDirectOperationEmitter<TEmitter>.Blt(EmitterLabel label) => Emit(OpCodes.Blt, label);
    public TEmitter Blt_S(EmitterLabel label) => Emit(OpCodes.Blt_S, label);
    public TEmitter Blt_Un(EmitterLabel label) => Emit(OpCodes.Blt_Un, label);
    public TEmitter Blt_Un_S(EmitterLabel label) => Emit(OpCodes.Blt_Un_S, label);
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

    TEmitter IDirectOperationEmitter<TEmitter>.Cgt() => Emit(OpCodes.Cgt);
    public TEmitter Cgt_Un() => Emit(OpCodes.Cgt_Un);
    TEmitter IDirectOperationEmitter<TEmitter>.Clt() => Emit(OpCodes.Clt);
    public TEmitter Clt_Un() => Emit(OpCodes.Clt_Un);
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
    TEmitter IDirectOperationEmitter<TEmitter>.Ldelem(Type type) => Emit(OpCodes.Ldelem, type);
    TEmitter IDirectOperationEmitter<TEmitter>.Ldelem<T>() => Ldelem(typeof(T));
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
    TEmitter IDirectOperationEmitter<TEmitter>.Ldelema(Type type) => Emit(OpCodes.Ldelema, type);
    TEmitter IDirectOperationEmitter<TEmitter>.Ldelema<T>() => Ldelema(typeof(T));
    TEmitter IDirectOperationEmitter<TEmitter>.Stelem(Type type) => Emit(OpCodes.Stelem, type);
    TEmitter IDirectOperationEmitter<TEmitter>.Stelem<T>() => Stelem(typeof(T));
    public TEmitter Stelem_I() => Emit(OpCodes.Stelem_I);
    public TEmitter Stelem_I1() => Emit(OpCodes.Stelem_I1);
    public TEmitter Stelem_I2() => Emit(OpCodes.Stelem_I2);
    public TEmitter Stelem_I4() => Emit(OpCodes.Stelem_I4);
    public TEmitter Stelem_I8() => Emit(OpCodes.Stelem_I8);
    public TEmitter Stelem_R4() => Emit(OpCodes.Stelem_R4);
    public TEmitter Stelem_R8() => Emit(OpCodes.Stelem_R8);
    public TEmitter Stelem_Ref() => Emit(OpCodes.Stelem_Ref);
    TEmitter IDirectOperationEmitter<TEmitter>.Ldfld(FieldInfo field) => Emit(OpCodes.Ldfld, field);
    public TEmitter Ldsfld(FieldInfo field) => Emit(OpCodes.Ldsfld, field);
    TEmitter IDirectOperationEmitter<TEmitter>.Ldflda(FieldInfo field) => Emit(OpCodes.Ldflda, field);
    public TEmitter Ldsflda(FieldInfo field) => Emit(OpCodes.Ldsflda, field);
    TEmitter IDirectOperationEmitter<TEmitter>.Stfld(FieldInfo field) => Emit(OpCodes.Stfld, field);
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
#endregion

    public override string ToString()
    {
        var text = StringBuilderPool.Rent();
        foreach (var line in this.Instructions)
        {
            text.Append(line).AppendLine();
        }
        text.Append($"IL_{this.Offset:X4}: <- Current Stream Position");
        return text.ToStringAndReturn();
    }

}

internal partial class Emitter<TEmitter> : ISimpleEmitter<TEmitter>
    //where TEmitter : ICleanEmitter<TEmitter>, IDirectEmitter<TEmitter>, ISimpleEmitter<TEmitter>
{
#region Simple
    public IArrayBuilder<TEmitter> Array => new ArrayBuilder<TEmitter>(this);
    public IBitwiseBuilder<TEmitter> Bitwise { get; }
    public IBranchBuilder<TEmitter> Branch { get; }
    public IValueInteractionBuilder<TEmitter> Value { get; }
    public IConvertBuilder<TEmitter> Convert { get; }
    public ICompareBuilder<TEmitter> Compare { get; }
    public ILoadBuilder<TEmitter> Load { get; }
    public IStoreBuilder<TEmitter> Store { get; }
    public ILabelBuilder<TEmitter> Label { get; }
    public ILocalBuilder<TEmitter> Local { get; }
    public IDebugBuilder<TEmitter> Debug { get; }
    public IExceptionBuilder<TEmitter> Exception { get; }
    public ITryCatchFinallyBuilder<TEmitter> Try(Action<TEmitter> emitTryBlock) => throw new NotImplementedException();
    public TEmitter Scoped(Action<TEmitter> emitScopedBlock) => throw new NotImplementedException();
    public TEmitter PushValue<T>(T? value) => throw new NotImplementedException();
    public TEmitter Return() => throw new NotImplementedException();
#endregion
}