// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

#if NETFRAMEWORK || NETSTANDARD2_0
using Polyfills;
#endif

using ScrubJay.Reflection.IL.Instructions;
using ScrubJay.Reflection.IL.LabelOffSetManagement;

namespace ScrubJay.Reflection.IL.Emission;

public abstract class EmitterBase
{
    protected readonly ILMethod _method;
    protected readonly ILGenerator _ilGenerator;

    protected readonly LabelManager _labels = new();
    private readonly Dictionary<ILLocal, LocalBuilder> _localMap = [];

    protected EmitterBase(ILMethod method, ILGenerator generator)
    {
        _method = method.ThrowIfNull();
        _ilGenerator = generator.ThrowIfNull();
    }

    protected virtual void AddInstruction(Instruction instr)
    {
        _method.AddInstruction(instr);
    }

    protected virtual void AddLocal(ILLocal local, LocalBuilder lb)
    {
        if (!_localMap.TryAdd(local, lb))
            throw new ArgumentException(null, nameof(local));
        _method._locals.Add(local);
    }

    protected virtual LocalBuilder GetLocal(ILLocal local)
    {
        if (!_localMap.TryGetValue(local, out var lb))
            throw new ArgumentException(null, nameof(local));
        return lb;
    }
}

public abstract class EmitterBase<E> : EmitterBase,
    IFluentBuilder<E>,
    IEmitter<E>,
    IOpCodeEmitter<E>,
    IGenEmitter<E>,
    IOperationEmitter<E>
    where E : IGenEmitter<E>, IOperationEmitter<E>, IOpCodeEmitter<E>
{
    protected readonly E _emitter;

    E IFluentBuilder<E>.Self => _emitter;

    public IInstructions Instructions => _method.Instructions;


    protected EmitterBase(DynamicILMethod method, ILGenerator ilGenerator)
        : base(method, ilGenerator)
    {
        _emitter = (E)(IEmitter<E>)this;
    }
    
    public E Invoke(Action<E>? instanceAction)
    {
        instanceAction?.Invoke(_emitter);
        return _emitter;
    }

    public E Invoke(Func<E, E>? instanceFluentFunc)
    {
        instanceFluentFunc?.Invoke(_emitter);
        return _emitter;
    }

#region IOpEmitter

    public E Emit(OpCode opCode)
    {
        if (opCode.OperandType != OperandType.InlineNone)
            throw new ArgumentException(null, nameof(opCode));

        _ilGenerator.Emit(opCode);
        AddInstruction(new OpCodeNoneInstruction(opCode));
        return _emitter;
    }

    public E Emit(OpCode opCode, byte u8)
    {
        //if (opCode.OperandType != Inlin)

        _ilGenerator.Emit(opCode, u8);

        Debugger.Break();
        return _emitter;
    }

    public E Emit(OpCode opCode, sbyte i8)
    {
        if (opCode.OperandType != OperandType.ShortInlineI)
            throw new ArgumentException(null, nameof(opCode));

        _ilGenerator.Emit(opCode, i8);
        Debugger.Break();
        return _emitter;
    }

    public E Emit(OpCode opCode, short i16)
    {
        _ilGenerator.Emit(opCode, i16);
        Debugger.Break();
        return _emitter;
    }

    public E Emit(OpCode opCode, int i32)
    {
        _ilGenerator.Emit(opCode, i32);
        AddInstruction(new OpCodeValueInstruction<int>(opCode, i32));
        return _emitter;
    }

    public E Emit(OpCode opCode, long i64)
    {
        _ilGenerator.Emit(opCode, i64);
        Debugger.Break();
        return _emitter;
    }

    public E Emit(OpCode opCode, float f32)
    {
        _ilGenerator.Emit(opCode, f32);
        Debugger.Break();
        return _emitter;
    }

    public E Emit(OpCode opCode, double i8)
    {
        _ilGenerator.Emit(opCode, i8);
        Debugger.Break();
        return _emitter;
    }

    public E Emit(OpCode opCode, string str)
    {
        Throw.IfNull(str);
        _ilGenerator.Emit(opCode, str);
        AddInstruction(new OpCodeStringInstruction(opCode, str));
        return _emitter;
    }

    public E Emit(OpCode opCode, ILLabel ilLabel)
    {
        Label label = _labels.Declared(ilLabel).SomeOrThrow("Specified ILLabel does not belong to this emitter");
        _ilGenerator.Emit(opCode, label);
        AddInstruction(new OpCodeLabelInstruction(opCode, ilLabel));
        return _emitter;
    }

    public E Emit(OpCode opCode, params ILLabel[] ilLabels)
    {
        Throw.IfEmpty(ilLabels);

        int count = ilLabels.Length;
        Label[] labels = new Label[count];
        for (int i = 0; i < count; i++)
        {
            var label = _labels.Declared(ilLabels[i]).SomeOrThrow("Specified ILLabel does not belong to this emitter");
            labels[i] = label;
        }

        _ilGenerator.Emit(opCode, labels);
        Debugger.Break();
        return _emitter;
    }

    public E Emit(OpCode opCode, ILLocal ilLocal)
    {
        var local = GetLocal(ilLocal);

        _ilGenerator.Emit(opCode, local);
        AddInstruction(new OpCodeLocalInstruction(opCode, ilLocal));
        return _emitter;
    }

    public E Emit(OpCode opCode, FieldInfo field)
    {
        Throw.IfNull(field);
        _ilGenerator.Emit(opCode, field);
        AddInstruction(new OpCodeFieldInstruction(opCode, field));
        return _emitter;
    }

    public E Emit(OpCode opCode, ConstructorInfo ctor)
    {
        if (opCode.OperandType != OperandType.InlineMethod)
            throw new ArgumentException(null, nameof(opCode));
        Throw.IfNull(ctor);

        _ilGenerator.Emit(opCode, ctor);
        Debugger.Break();
        return _emitter;
    }

    public E Emit(OpCode opCode, MethodInfo method)
    {
        Throw.IfNull(method);
        _ilGenerator.Emit(opCode, method);
        AddInstruction(new OpCodeMethodInstruction(opCode, method.MetadataToken)
        {
            Method = method,
        });
        return _emitter;
    }

    public E Emit(OpCode opCode, Type type)
    {
        Throw.IfNull(type);
        _ilGenerator.Emit(opCode, type);
        AddInstruction(new OpCodeTypeInstruction(opCode, type.MetadataToken)
        {
            Type = type,
        });
        return _emitter;
    }

    public E Emit(OpCode opCode, SignatureHelper signature)
    {
        Throw.IfNull(signature);
        _ilGenerator.Emit(opCode, signature);
        Debugger.Break();
        return _emitter;
    }

#endregion

#region IGenEmitter

    public E BeginExceptionBlock(out ILLabel label,
        [CallerArgumentExpression(nameof(label))]
        string? labelName = null)
    {
        Label lbl = _ilGenerator.BeginExceptionBlock();
        label = _labels.Declare(lbl, labelName);
        AddInstruction(new ILGeneratorBeginExceptionBlockInstruction(label));
        return _emitter;
    }

    public E BeginCatchBlock(Type exceptionType)
    {
        Throw.IfNull(exceptionType);
        if (!exceptionType.Implements<Exception>())
            throw new ArgumentException($"Exception Type '{exceptionType.Render()}' is not an Exception", nameof(exceptionType));
        _ilGenerator.BeginCatchBlock(exceptionType);
        AddInstruction(new ILGeneratorBeginCatchBlockInstruction(exceptionType));
        return _emitter;
    }

    public E BeginCatchBlock<TException>()
        where TException : Exception
        => BeginCatchBlock(typeof(TException));

    public E BeginFinallyBlock()
    {
        _ilGenerator.BeginFinallyBlock();
        AddInstruction(new ILGeneratorInstruction(ILGeneratorMethod.BeginFinallyBlock));
        return _emitter;
    }

    public E BeginExceptFilterBlock()
    {
        _ilGenerator.BeginExceptFilterBlock();
        AddInstruction(new ILGeneratorInstruction(ILGeneratorMethod.BeginExceptFilterBlock));
        return _emitter;
    }

    public E BeginFaultBlock()
    {
        _ilGenerator.BeginFaultBlock();
        AddInstruction(new ILGeneratorInstruction(ILGeneratorMethod.BeginFaultBlock));
        return _emitter;
    }

    public E EndExceptionBlock()
    {
        _ilGenerator.EndExceptionBlock();
        AddInstruction(new ILGeneratorInstruction(ILGeneratorMethod.EndExceptionBlock));
        return _emitter;
    }

    public E BeginScope()
    {
        _ilGenerator.BeginScope();
        AddInstruction(new ILGeneratorInstruction(ILGeneratorMethod.BeginScope));
        return _emitter;
    }

    public E EndScope()
    {
        _ilGenerator.EndScope();
        AddInstruction(new ILGeneratorInstruction(ILGeneratorMethod.EndScope));
        return _emitter;
    }

    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/namespaces#143-namespace-declarations"/>
    public E UsingNamespace(string @namespace)
    {
        Throw.IfEmpty(@namespace);
        if (!CodeHelper.IsValidNamespace(@namespace))
            throw new ArgumentException($"Namespace '{@namespace}' is not valid", nameof(@namespace));
        _ilGenerator.UsingNamespace(@namespace);
        AddInstruction(new ILGeneratorUsingNamespaceInstruction(@namespace));
        return _emitter;
    }

    public E DeclareLocal(Type localType, out ILLocal local,
        [CallerArgumentExpression(nameof(local))]
        string? localName = null)
    {
        var localBuilder = _ilGenerator.DeclareLocal(localType);
        local = new ILLocal(localBuilder, EmissionHelper.GetLocalName(localName));
        AddLocal(local, localBuilder);
        AddInstruction(new ILGeneratorDeclareLocalInstruction(local));
        return _emitter;
    }

    public E DeclareLocal<T>(out ILLocal local,
        [CallerArgumentExpression(nameof(local))]
        string? localName = null)
        => DeclareLocal(typeof(T), out local, localName);

    public E DeclareLocal(Type localType, bool pinned, out ILLocal local,
        [CallerArgumentExpression(nameof(local))]
        string? localName = null)
    {
        var localBuilder = _ilGenerator!.DeclareLocal(localType, pinned);
        local = new ILLocal(localBuilder, EmissionHelper.GetLocalName(localName));
        AddLocal(local, localBuilder);
        AddInstruction(new ILGeneratorDeclareLocalInstruction(local));
        return _emitter;
    }

    public E DeclareLocal<T>(bool pinned, out ILLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null)
        => DeclareLocal(typeof(T), pinned, out local, localName);

    public E DefineLabel(out ILLabel label,
        [CallerArgumentExpression(nameof(label))]
        string? labelName = null)
    {
        var lbl = _ilGenerator.DefineLabel();
        label = _labels.Declare(lbl, labelName);
        AddInstruction(new ILGeneratorDefineLabelInstruction(label));
        return _emitter;
    }

    public E MarkLabel(ILLabel label)
    {
        var lbl = _labels.Declared(label).SomeOrThrow();
        label.Offset = _ilGenerator.ILOffset;
        _ilGenerator.MarkLabel(lbl);
        AddInstruction(new ILGeneratorMarkLabelInstruction(label));
        return _emitter;
    }

    public E EmitCall(MethodInfo methodInfo, Type[]? optionalParameterTypes = null)
    {
        Throw.IfNull(methodInfo);
        var callOpCode = methodInfo.GetCallOpCode();

        _ilGenerator.EmitCall(callOpCode, methodInfo, optionalParameterTypes);
        AddInstruction(new ILGeneratorCallVarargsInstruction(callOpCode, methodInfo, optionalParameterTypes));
        return _emitter;
    }

    public E EmitCalli(
        CallingConventions callingConventions,
        Type? returnType,
        Type[]? parameterTypes,
        Type[]? optionalParameterTypes = null)
    {
        _ilGenerator.EmitCalli(
            OpCodes.Calli,
            callingConventions, returnType, parameterTypes, optionalParameterTypes);
        AddInstruction(new ILGeneratorCallManagedInstruction(callingConventions, returnType, parameterTypes, optionalParameterTypes));
        return _emitter;
    }

#if !NETSTANDARD2_0
    public E EmitCalli(CallingConvention unmanagedCallConv, Type? returnType, Type[]? parameterTypes)
    {
        _ilGenerator.EmitCalli(
            OpCodes.Calli,
            unmanagedCallConv, returnType, parameterTypes);
        AddInstruction(new ILGeneratorCallUnmanagedInstruction(unmanagedCallConv, returnType, parameterTypes));
        return _emitter;
    }
#endif

#endregion

    public override string ToString()
    {
//        using var text = new TextBuilder();
//        text.Delimit(static tb => tb.NewLine(), _method.Instructions, static (tb, instr) => tb.Append(instr));
//        return text.ToString();
        return _method.Instructions.ToString() ?? "WTF?";
    }

#region IOperationEmitter

#region Math Operators

    public E Add() => Emit(OpCodes.Add);

    public E Add_Ovf() => Emit(OpCodes.Add_Ovf);

    public E Add_Ovf_Un() => Emit(OpCodes.Add_Ovf_Un);

    public E Div() => Emit(OpCodes.Div);

    public E Div_Un() => Emit(OpCodes.Div_Un);

    public E Mul() => Emit(OpCodes.Mul);

    public E Mul_Ovf() => Emit(OpCodes.Mul_Ovf);

    public E Mul_Ovf_Un() => Emit(OpCodes.Mul_Ovf_Un);

    public E Rem() => Emit(OpCodes.Rem);

    public E Rem_Un() => Emit(OpCodes.Rem_Un);

    public E Sub() => Emit(OpCodes.Sub);

    public E Sub_Ovf() => Emit(OpCodes.Sub_Ovf);

    public E Sub_Ovf_Un() => Emit(OpCodes.Sub_Ovf_Un);

#endregion

#region Bitwise Operators

    public E And() => Emit(OpCodes.And);

    public E Neg() => Emit(OpCodes.Neg);

    public E Not() => Emit(OpCodes.Not);

    public E Or() => Emit(OpCodes.Or);

    public E Shl() => Emit(OpCodes.Shl);

    public E Shr() => Emit(OpCodes.Shr);

    public E Shr_Un() => Emit(OpCodes.Shr_Un);

    public E Xor() => Emit(OpCodes.Xor);

#endregion

#region Method related

    public E Arglist() => Emit(OpCodes.Arglist);

    public E Call(MethodInfo method) => Emit(method.GetCallOpCode(), method);

    public E Callvirt(MethodInfo method) => Call(method);

    public E Constrained(Type type) => Emit(OpCodes.Constrained, type);

    public E Constrained<T>() => Constrained(typeof(T));

    public E Ldftn(MethodInfo method) => Emit(OpCodes.Ldftn, method);

    public E Ldvirtftn(MethodInfo method) => Emit(OpCodes.Ldvirtftn, method);

    public E Tailcall() => Emit(OpCodes.Tailcall);

#endregion

#region Branching

#region Comparison
    public E Brtrue(ILLabel label) => Emit(OpCodes.Brtrue, label);
    public E Brtrue_S(ILLabel label) => Brtrue(label);
    public E Brfalse(ILLabel label) => Emit(OpCodes.Brfalse, label);
    public E Brfalse_S(ILLabel label) => Brfalse(label);
    
    public E Beq(ILLabel label) => Emit(OpCodes.Beq, label);
    public E Beq_S(ILLabel label) => Beq(label);
    public E Bne_Un(ILLabel label) => Emit(OpCodes.Bne_Un, label);
    public E Bne_Un_S(ILLabel label) => Bne_Un(label);
    
    public E Bgt(ILLabel label) => Emit(OpCodes.Bgt, label);
    public E Bgt_S(ILLabel label) => Bgt(label);
    public E Bgt_Un(ILLabel label) => Emit(OpCodes.Bgt_Un, label);
    public E Bgt_Un_S(ILLabel label) => Bgt_Un(label);
    
    public E Bge(ILLabel label) => Emit(OpCodes.Bge, label);
    public E Bge_S(ILLabel label) => Bge(label);
    public E Bge_Un(ILLabel label) => Emit(OpCodes.Bge_Un, label);
    public E Bge_Un_S(ILLabel label) => Bge_Un(label);
    
    public E Blt(ILLabel label) => Emit(OpCodes.Blt, label);
    public E Blt_S(ILLabel label) => Blt(label);
    public E Blt_Un(ILLabel label) => Emit(OpCodes.Blt_Un, label);
    public E Blt_Un_S(ILLabel label) => Blt_Un(label);
    
    public E Ble(ILLabel label) => Emit(OpCodes.Ble, label);
    public E Ble_S(ILLabel label) => Ble(label);
    public E Ble_Un(ILLabel label) => Emit(OpCodes.Ble_Un, label);
    public E Ble_Un_S(ILLabel label) => Ble_Un(label);

    public E Branch(ILLabel label) => Br(label);

    public E Branch(out ILLabel label,
        [CallerArgumentExpression(nameof(label))]
        string? labelName = null)
        => DefineLabel(out label, labelName).Br(label);
    
    public E Branch(CompareOp comparison, ILLabel label, bool unsigned = false)
    {
        if (comparison is CompareOp.NotEqual or (CompareOp.LessThan | CompareOp.GreaterThan))
            return Bne_Un(label);
        if (comparison == CompareOp.Equal)
            return Beq(label);
        if (comparison == CompareOp.LessThan)
            return unsigned ? Blt_Un(label) : Blt(label);
        if (comparison == CompareOp.LessThanOrEqual)
            return unsigned ? Ble_Un(label) : Ble(label);
        if (comparison == CompareOp.GreaterThan)
            return unsigned ? Bgt_Un(label) : Bgt(label);
        if (comparison == CompareOp.GreaterThanOrEqual)
            return unsigned ?  Bge_Un(label) : Bge(label);
        if (comparison == CompareOp.Unconditional)
            return Br(label);
        throw InvalidEnumException.Create(comparison);
    }

    public E Branch(CompareOp comparison, out ILLabel label, bool unsigned = false,
        [CallerArgumentExpression(nameof(label))]
        string? labelName = null)
    {
        DefineLabel(out label, labelName);
        Branch(comparison, label, unsigned);
        return _emitter;
    }

    public E Branch(bool boolean, ILLabel label)
    {
        if (boolean)
            return Brtrue(label);
        return Brfalse(label);
    }
    
    public E Branch(bool boolean, out ILLabel label,
        [CallerArgumentExpression(nameof(label))]
        string? labelName = null)
    {
        DefineLabel(out label, labelName); 
        Branch(boolean, label);
        return _emitter;
    }

#endregion

#region Unconditional

    public E Br(ILLabel label)
    {
        return Emit(OpCodes.Br, label);
    }

    public E Br_S(ILLabel label) => Br(label);

    public E Leave(ILLabel label)
    {
        return Emit(OpCodes.Leave, label);
    }

    public E Leave_S(ILLabel label) => Leave(label);

#endregion

    public E Jmp(MethodInfo method) => Emit(OpCodes.Jmp, method);

    public E Ret() => Emit(OpCodes.Ret);

    public E Switch(params ILLabel[] labels) => Emit(OpCodes.Switch, labels);

#endregion

#region Boxing, Unboxing, Casting

    public E Box(Type type) => Emit(OpCodes.Box, type);

    public E Box<T>()
        => Box(typeof(T));

    public E Castclass(Type type) => Emit(OpCodes.Castclass, type);

    public E Castclass<T>() where T : class
        => Castclass(typeof(T));

    public E Isinst(Type type) => Emit(OpCodes.Isinst, type);

    public E Isinst<T>()
        => Isinst(typeof(T));

    public E Unbox(Type type) => Emit(OpCodes.Unbox, type);

    public E Unbox<T>()
        => Unbox(typeof(T));

    public E Unbox_Any(Type type) => Emit(OpCodes.Unbox_Any, type);

    public E Unbox_Any<T>()
        => Unbox_Any(typeof(T));

#endregion

#region Debugging

    public E Break() => Emit(OpCodes.Break);

    public E Nop() => Emit(OpCodes.Nop);

#endregion

#region Comparison

    public E Ceq() => Emit(OpCodes.Ceq);

    public E Cgt() => Emit(OpCodes.Cgt);

    public E Cgt_Un() => Emit(OpCodes.Cgt_Un);

    public E Clt() => Emit(OpCodes.Clt);

    public E Clt_Un() => Emit(OpCodes.Clt_Un);

#endregion

#region Exceptions

    public E Ckfinite() => Emit(OpCodes.Ckfinite);

    public E Endfilter() => Emit(OpCodes.Endfilter);

    public E Endfinally() => Emit(OpCodes.Endfinally);

    public E Rethrow() => Emit(OpCodes.Rethrow);

    E IOperationEmitter<E>.Throw() => Emit(OpCodes.Throw);

#endregion

#region Value Conversion

    public E Conv_I() => Emit(OpCodes.Conv_I);

    public E Conv_Ovf_I() => Emit(OpCodes.Conv_Ovf_I);

    public E Conv_Ovf_I_Un() => Emit(OpCodes.Conv_Ovf_I_Un);

    public E Conv_I1() => Emit(OpCodes.Conv_I1);

    public E Conv_Ovf_I1() => Emit(OpCodes.Conv_Ovf_I1);

    public E Conv_Ovf_I1_Un() => Emit(OpCodes.Conv_Ovf_I1_Un);

    public E Conv_I2() => Emit(OpCodes.Conv_I2);

    public E Conv_Ovf_I2() => Emit(OpCodes.Conv_Ovf_I2);

    public E Conv_Ovf_I2_Un() => Emit(OpCodes.Conv_Ovf_I2_Un);

    public E Conv_I4() => Emit(OpCodes.Conv_I4);

    public E Conv_Ovf_I4() => Emit(OpCodes.Conv_Ovf_I4);

    public E Conv_Ovf_I4_Un() => Emit(OpCodes.Conv_Ovf_I4_Un);

    public E Conv_I8() => Emit(OpCodes.Conv_I8);

    public E Conv_Ovf_I8() => Emit(OpCodes.Conv_Ovf_I8);

    public E Conv_Ovf_I8_Un() => Emit(OpCodes.Conv_Ovf_I8_Un);

    public E Conv_U() => Emit(OpCodes.Conv_U);

    public E Conv_Ovf_U() => Emit(OpCodes.Conv_Ovf_U);

    public E Conv_Ovf_U_Un() => Emit(OpCodes.Conv_Ovf_U_Un);

    public E Conv_U1() => Emit(OpCodes.Conv_U1);

    public E Conv_Ovf_U1() => Emit(OpCodes.Conv_Ovf_U1);

    public E Conv_Ovf_U1_Un() => Emit(OpCodes.Conv_Ovf_U1_Un);

    public E Conv_U2() => Emit(OpCodes.Conv_U2);

    public E Conv_Ovf_U2() => Emit(OpCodes.Conv_Ovf_U2);

    public E Conv_Ovf_U2_Un() => Emit(OpCodes.Conv_Ovf_U2_Un);

    public E Conv_U4() => Emit(OpCodes.Conv_U4);

    public E Conv_Ovf_U4() => Emit(OpCodes.Conv_Ovf_U4);

    public E Conv_Ovf_U4_Un() => Emit(OpCodes.Conv_Ovf_U4_Un);

    public E Conv_U8() => Emit(OpCodes.Conv_U8);

    public E Conv_Ovf_U8() => Emit(OpCodes.Conv_Ovf_U8);

    public E Conv_Ovf_U8_Un() => Emit(OpCodes.Conv_Ovf_U8_Un);

    public E Conv_R_Un() => Emit(OpCodes.Conv_R_Un);

    public E Conv_R4() => Emit(OpCodes.Conv_R4);

    public E Conv_R8() => Emit(OpCodes.Conv_R8);

#endregion

#region *byte

    public E Cpblk() => Emit(OpCodes.Cpblk);

    public E Initblk() => Emit(OpCodes.Initblk);

    public E Localloc() => Emit(OpCodes.Localloc);

#endregion

#region Stack Manip.

    public E Dup() => Emit(OpCodes.Dup);
    public E Pop() => Emit(OpCodes.Pop);

#endregion

#region Create/Init

    public E Initobj(Type type) => Emit(OpCodes.Initobj, type);

    public E Initobj<T>() where T : struct
        => Initobj(typeof(T));

    public E Newobj(ConstructorInfo ctor) => Emit(OpCodes.Newobj, ctor);

#endregion

#region Args/Params

#region Load Argument

    public E Ldarg_0() => Emit(OpCodes.Ldarg_0);

    public E Ldarg_1() => Emit(OpCodes.Ldarg_1);

    public E Ldarg_2() => Emit(OpCodes.Ldarg_2);

    public E Ldarg_3() => Emit(OpCodes.Ldarg_3);

    public E Ldarg_S(byte index)
    {
        return index switch
        {
            0 => Emit(OpCodes.Ldarg_0),
            1 => Emit(OpCodes.Ldarg_1),
            2 => Emit(OpCodes.Ldarg_2),
            3 => Emit(OpCodes.Ldarg_3),
            _ => Emit(OpCodes.Ldarg_S, index),
        };
    }

    public E Ldarg(ushort index)
    {
        return index switch
        {
            0 => Emit(OpCodes.Ldarg_0),
            1 => Emit(OpCodes.Ldarg_1),
            2 => Emit(OpCodes.Ldarg_2),
            3 => Emit(OpCodes.Ldarg_3),
            <= byte.MaxValue => Emit(OpCodes.Ldarga_S, (byte)index),
            _ => Emit(OpCodes.Ldarg, index),
        };
    }
    
    public E Ldarg(int index)
    {
        return index switch
        {
            0 => Emit(OpCodes.Ldarg_0),
            1 => Emit(OpCodes.Ldarg_1),
            2 => Emit(OpCodes.Ldarg_2),
            3 => Emit(OpCodes.Ldarg_3),
            >= byte.MinValue and <= byte.MaxValue => Emit(OpCodes.Ldarga_S, (byte)index),
            >= ushort.MinValue and <= ushort.MaxValue => Emit(OpCodes.Ldarg, index),
            _ => throw new ArgumentException(null, nameof(index)),
        };
    }

    public E Ldarg(ParameterInfo parameter)
        => Ldarg(parameter.Position);

    public E Ldarga(ushort index)
    {
        if (index <= byte.MaxValue)
            return Emit(OpCodes.Ldarga_S, (byte)index);
        return Emit(OpCodes.Ldarga, index);
    }
    
    public E Ldarga(int index)
    {
        if (index >= byte.MinValue && index <= byte.MaxValue)
            return Emit(OpCodes.Ldarga_S, (byte)index);
        if (index >= ushort.MinValue && index <= ushort.MaxValue)
            return Emit(OpCodes.Ldarga, index);
        throw new ArgumentException(null, nameof(index));
    }

    public E Ldarga_S(byte index) => Emit(OpCodes.Ldarga_S, index);

#endregion

#region Store in Argument

    public E Starg(ushort index)
    {
        if (index <= byte.MaxValue)
            return Emit(OpCodes.Starg_S, (byte)index);
        return Emit(OpCodes.Starg, index);
    }
    
    public E Starg(int index)
    {
        if (index >= byte.MinValue && index <= byte.MaxValue)
            return Emit(OpCodes.Starg_S, (byte)index);
        if (index >= ushort.MinValue && index <= ushort.MaxValue)
            return Emit(OpCodes.Starg, index);
        throw new ArgumentException(null, nameof(index));
    }

    public E Starg_S(byte index) => Emit(OpCodes.Starg_S, index);

#endregion

#endregion

#region Load Constant|Value

    public E Ldc_I4_M1() => Emit(OpCodes.Ldc_I4_M1);

    public E Ldc_I4_0() => Emit(OpCodes.Ldc_I4_0);

    public E Ldc_I4_1() => Emit(OpCodes.Ldc_I4_1);

    public E Ldc_I4_2() => Emit(OpCodes.Ldc_I4_2);

    public E Ldc_I4_3() => Emit(OpCodes.Ldc_I4_3);

    public E Ldc_I4_4() => Emit(OpCodes.Ldc_I4_4);

    public E Ldc_I4_5() => Emit(OpCodes.Ldc_I4_5);

    public E Ldc_I4_6() => Emit(OpCodes.Ldc_I4_6);

    public E Ldc_I4_7() => Emit(OpCodes.Ldc_I4_7);

    public E Ldc_I4_8() => Emit(OpCodes.Ldc_I4_8);

    public E Ldc_I4_S(sbyte value)
    {
        return value switch
        {
            -1 => Ldc_I4_M1(),
            0 => Ldc_I4_0(),
            1 => Ldc_I4_1(),
            2 => Ldc_I4_2(),
            3 => Ldc_I4_3(),
            4 => Ldc_I4_4(),
            5 => Ldc_I4_5(),
            6 => Ldc_I4_6(),
            7 => Ldc_I4_7(),
            8 => Ldc_I4_8(),
            _ => Emit(OpCodes.Ldc_I4_S, value),
        };
    }

    public E Ldc_I4(int value)
    {
        return value switch
        {
            -1 => Ldc_I4_M1(),
            0 => Ldc_I4_0(),
            1 => Ldc_I4_1(),
            2 => Ldc_I4_2(),
            3 => Ldc_I4_3(),
            4 => Ldc_I4_4(),
            5 => Ldc_I4_5(),
            6 => Ldc_I4_6(),
            7 => Ldc_I4_7(),
            8 => Ldc_I4_8(),
            >= sbyte.MinValue and <= sbyte.MaxValue => Emit(OpCodes.Ldc_I4_S, (sbyte)value),
            _ => Emit(OpCodes.Ldc_I4, value),
        };
    }

    public E Ldc_I8(long value) => Emit(OpCodes.Ldc_I8, value);

    public E Ldc_R4(float value) => Emit(OpCodes.Ldc_R4, value);

    public E Ldc_R8(double value) => Emit(OpCodes.Ldc_R8, value);

    public E Ldnull() => Emit(OpCodes.Ldnull);

    public E Ldstr(string str) => Emit(OpCodes.Ldstr, str);

#endregion

#region Load Token

    public E Ldtoken(Type type) => Emit(OpCodes.Ldtoken, type);

    public E Ldtoken(FieldInfo field) => Emit(OpCodes.Ldtoken, field);

    public E Ldtoken(MethodInfo method) => Emit(OpCodes.Ldtoken, method);

#endregion

#region Arrays

    public E Ldlen() => Emit(OpCodes.Ldlen);

    public E Newarr(Type type) => Emit(OpCodes.Newarr, type);

    public E Newarr<T>()
        => Newarr(typeof(T));

    public E Readonly() => Emit(OpCodes.Readonly);

#region Load array Element

    public E Ldelem_I() => Emit(OpCodes.Ldelem_I);

    public E Ldelem_I1() => Emit(OpCodes.Ldelem_I1);

    public E Ldelem_I2() => Emit(OpCodes.Ldelem_I2);

    public E Ldelem_I4() => Emit(OpCodes.Ldelem_I4);

    public E Ldelem_I8() => Emit(OpCodes.Ldelem_I8);

    public E Ldelem_U1() => Emit(OpCodes.Ldelem_U1);

    public E Ldelem_U2() => Emit(OpCodes.Ldelem_U2);

    public E Ldelem_U4() => Emit(OpCodes.Ldelem_U4);

    public E Ldelem_R4() => Emit(OpCodes.Ldelem_R4);

    public E Ldelem_R8() => Emit(OpCodes.Ldelem_R8);

    public E Ldelem_Ref() => Emit(OpCodes.Ldelem_Ref);

    public E Ldelem(Type type)
    {
        if (type == typeof(nint) || type == typeof(IntPtr))
            return Ldelem_I();
        if (type == typeof(sbyte))
            return Ldelem_I1();
        if (type == typeof(short))
            return Ldelem_I2();
        if (type == typeof(int))
            return Ldelem_I4();
        if (type == typeof(long))
            return Ldelem_I8();
        if (type == typeof(byte))
            return Ldelem_U1();
        if (type == typeof(ushort))
            return Ldelem_U2();
        if (type == typeof(uint))
            return Ldelem_U4();
        if (type == typeof(float))
            return Ldelem_R4();
        if (type == typeof(double))
            return Ldelem_R8();
        if (type == typeof(object))
            return Ldelem_Ref();
        return Emit(OpCodes.Ldelem, type);
    }

    public E Ldelem<T>() => Ldelem(typeof(T));

    public E Ldelema(Type type) => Emit(OpCodes.Ldelema, type);

    public E Ldelema<T>() => Ldelema(typeof(T));

#endregion

#region Store in array Element

    public E Stelem_I() => Emit(OpCodes.Stelem_I);

    public E Stelem_I1() => Emit(OpCodes.Stelem_I1);

    public E Stelem_I2() => Emit(OpCodes.Stelem_I2);

    public E Stelem_I4() => Emit(OpCodes.Stelem_I4);

    public E Stelem_I8() => Emit(OpCodes.Stelem_I8);

    public E Stelem_R4() => Emit(OpCodes.Stelem_R4);

    public E Stelem_R8() => Emit(OpCodes.Stelem_R8);

    public E Stelem_Ref() => Emit(OpCodes.Stelem_Ref);

    public E Stelem(Type type)
    {
        if (type == typeof(nint) || type == typeof(IntPtr))
            return Stelem_I();
        if (type == typeof(sbyte))
            return Stelem_I1();
        if (type == typeof(short))
            return Stelem_I2();
        if (type == typeof(int))
            return Stelem_I4();
        if (type == typeof(long))
            return Stelem_I8();
//        if (type == typeof(byte))
//            return Stelem_U1();
//        if (type == typeof(ushort))
//            return Stelem_U2();
//        if (type == typeof(uint))
//            return Stelem_U4();
        if (type == typeof(float))
            return Stelem_R4();
        if (type == typeof(double))
            return Stelem_R8();
        if (type == typeof(object))
            return Stelem_Ref();
        return Emit(OpCodes.Stelem, type);
    }

    public E Stelem<T>() => Stelem(typeof(T));

#endregion

#endregion

#region Load|Store in Field

    public E Ldfld(FieldInfo field)
    {
        //MemberAssert.IsInstance(field);
        if (field.IsStatic)
            return Emit(OpCodes.Ldsfld, field);
        return Emit(OpCodes.Ldfld, field);
    }
    
    public E Ldflda(FieldInfo field)
    {
        //MemberAssert.IsInstance(field);
        if (field.IsStatic)
            return Emit(OpCodes.Ldsflda, field);
        return Emit(OpCodes.Ldflda, field);
    }

    public E Ldsfld(FieldInfo field)
    {
        //MemberAssert.IsStatic(field);
        if (field.IsStatic)
            return Emit(OpCodes.Ldsfld, field);
        return Emit(OpCodes.Ldfld, field);
    }

    public E Ldsflda(FieldInfo field)
    {
        //MemberAssert.IsStatic(field);
        if (field.IsStatic)
            return Emit(OpCodes.Ldsflda, field);
        return Emit(OpCodes.Ldflda, field);
    }
   
    public E Stfld(FieldInfo field)
    {
        //MemberAssert.IsInstance(field);
        if (field.IsStatic)
            return Emit(OpCodes.Stsfld, field);
        return Emit(OpCodes.Stfld, field);
    }

    public E Stsfld(FieldInfo field)
    {
        //MemberAssert.IsStatic(field);
        if (field.IsStatic)
            return Emit(OpCodes.Stsfld, field);
        return Emit(OpCodes.Stfld, field);
    }

#endregion

#region Addressing

    public E Cpobj(Type type) => Emit(OpCodes.Cpobj, type);

    public E Cpobj<T>()
        => Cpobj(typeof(T));

    public E Unaligned(int alignment)
    {
        if (alignment is not (1 or 2 or 4))
            throw new ArgumentOutOfRangeException(nameof(alignment), alignment, "Alignment must be 1, 2, or 4");
        return Emit(OpCodes.Unaligned, alignment);
    }

    public E Volatile() => Emit(OpCodes.Volatile);

#region Load

    public E Ldind_I() => Emit(OpCodes.Ldind_I);

    public E Ldind_I1() => Emit(OpCodes.Ldind_I1);

    public E Ldind_I2() => Emit(OpCodes.Ldind_I2);

    public E Ldind_I4() => Emit(OpCodes.Ldind_I4);

    public E Ldind_I8() => Emit(OpCodes.Ldind_I8);

    public E Ldind_U1() => Emit(OpCodes.Ldind_U1);

    public E Ldind_U2() => Emit(OpCodes.Ldind_U2);

    public E Ldind_U4() => Emit(OpCodes.Ldind_U4);

    public E Ldind_R4() => Emit(OpCodes.Ldind_R4);

    public E Ldind_R8() => Emit(OpCodes.Ldind_R8);

    public E Ldind_Ref() => Emit(OpCodes.Ldind_Ref);

    public E Ldind(Type type)
    {
        if (type == typeof(IntPtr) || type == typeof(nint))
            return Emit(OpCodes.Ldind_I);
        if (type == typeof(sbyte))
            return Emit(OpCodes.Ldind_I1);
        if (type == typeof(short))
            return Emit(OpCodes.Ldind_I2);
        if (type == typeof(int))
            return Emit(OpCodes.Ldind_I4);
        if (type == typeof(long))
            return Emit(OpCodes.Ldind_I8);
        if (type == typeof(byte))
            return Emit(OpCodes.Ldind_U1);
        if (type == typeof(ushort))
            return Emit(OpCodes.Ldind_U2);
        if (type == typeof(uint))
            return Emit(OpCodes.Ldind_U4);
        if (type == typeof(float))
            return Emit(OpCodes.Ldind_R4);
        if (type == typeof(double))
            return Emit(OpCodes.Ldind_R8);
        if (type == typeof(object))
            return Emit(OpCodes.Ldind_Ref);
        return Emit(OpCodes.Ldobj, type);
    }
    public E Ldind<T>() => Ldind(typeof(T));
    
    public E Ldobj(Type type) => Emit(OpCodes.Ldobj, type);

    public E Ldobj<T>() => Ldobj(typeof(T));

#endregion

#region Store

    public E Stind_I() => Emit(OpCodes.Stind_I);

    public E Stind_I1() => Emit(OpCodes.Stind_I1);

    public E Stind_I2() => Emit(OpCodes.Stind_I2);

    public E Stind_I4() => Emit(OpCodes.Stind_I4);

    public E Stind_I8() => Emit(OpCodes.Stind_I8);

    public E Stind_R4() => Emit(OpCodes.Stind_R4);

    public E Stind_R8() => Emit(OpCodes.Stind_R8);

    public E Stind_Ref() => Emit(OpCodes.Stind_Ref);

    public E Stind(Type type)
    {
        if (type == typeof(IntPtr) || type == typeof(nint))
            return Emit(OpCodes.Stind_I);
        if (type == typeof(sbyte))
            return Emit(OpCodes.Stind_I1);
        if (type == typeof(short))
            return Emit(OpCodes.Stind_I2);
        if (type == typeof(int))
            return Emit(OpCodes.Stind_I4);
        if (type == typeof(long))
            return Emit(OpCodes.Stind_I8);
        if (type == typeof(float))
            return Emit(OpCodes.Stind_R4);
        if (type == typeof(double))
            return Emit(OpCodes.Stind_R8);
        if (type == typeof(object))
            return Emit(OpCodes.Stind_Ref);
        return Emit(OpCodes.Ldobj, type);
    }
    public E Stind<T>() => Stind(typeof(T));
    
    public E Stobj(Type type) => Emit(OpCodes.Stobj, type);

    public E Stobj<T>() => Stobj(typeof(T));
#endregion

#endregion

#region Locals

#region Load Local

    public E Ldloc_0() => Emit(OpCodes.Ldloc_0);

    public E Ldloc_1() => Emit(OpCodes.Ldloc_1);

    public E Ldloc_2() => Emit(OpCodes.Ldloc_2);

    public E Ldloc_3() => Emit(OpCodes.Ldloc_3);

    public E Ldloc_S(byte index) => Emit(OpCodes.Ldloc_S, index);

    public E Ldloc(ushort index)
    {
        return index switch
        {
            0 => Emit(OpCodes.Ldloc_0),
            1 => Emit(OpCodes.Ldloc_1),
            2 => Emit(OpCodes.Ldloc_2),
            3 => Emit(OpCodes.Ldloc_3),
            <= byte.MaxValue => Emit(OpCodes.Ldloc_S, (byte)index),
            _ => Emit(OpCodes.Ldloc, index),
        };
    }

    public E Ldloc(ILLocal local)
    {
        return local.Index switch
        {
            0 => Emit(OpCodes.Ldloc_0),
            1 => Emit(OpCodes.Ldloc_1),
            2 => Emit(OpCodes.Ldloc_2),
            3 => Emit(OpCodes.Ldloc_3),
            <= byte.MaxValue => Emit(OpCodes.Ldloc_S, local),
            _ => Emit(OpCodes.Ldloc, local),
        };
    }

    public E Ldloc_S(ILLocal local) => Ldloc(local);

    public E Ldloca(ILLocal local)
    {
        if (local.IsShortForm)
            return Emit(OpCodes.Ldloca_S, local);
        return Emit(OpCodes.Ldloca, local);
    }

    public E Ldloca_S(ILLocal local) => Ldloca(local);

#endregion

#region Store Local

    public E Stloc_0() => Emit(OpCodes.Stloc_0);
    public E Stloc_1() => Emit(OpCodes.Stloc_1);
    public E Stloc_2() => Emit(OpCodes.Stloc_2);
    public E Stloc_3() => Emit(OpCodes.Stloc_3);

    public E Stloc_S(byte index)
    {
        return index switch
        {
            0 => Emit(OpCodes.Stloc_0),
            1 => Emit(OpCodes.Stloc_1),
            2 => Emit(OpCodes.Stloc_2),
            3 => Emit(OpCodes.Stloc_3),
            _ => Emit(OpCodes.Stloc_S, index),
        };
    }

    public E Stloc(ushort index)
    {
        return index switch
        {
            0 => Emit(OpCodes.Stloc_0),
            1 => Emit(OpCodes.Stloc_1),
            2 => Emit(OpCodes.Stloc_2),
            3 => Emit(OpCodes.Stloc_3),
            <= byte.MaxValue => Emit(OpCodes.Stloc_S, (byte)index),
            _ => Emit(OpCodes.Stloc, index),
        };
    }


    public E Stloc(ILLocal local)
    {
        return local.Index switch
        {
            0 => Emit(OpCodes.Stloc_0),
            1 => Emit(OpCodes.Stloc_1),
            2 => Emit(OpCodes.Stloc_2),
            3 => Emit(OpCodes.Stloc_3),
            <= byte.MaxValue => Emit(OpCodes.Stloc_S, local),
            _ => Emit(OpCodes.Stloc, local),
        };
    }

    public E Stloc_S(ILLocal local) => Stloc(local);

#endregion

#endregion

#region Type/Ref

    public E Mkrefany(Type type) => Emit(OpCodes.Mkrefany, type);

    public E Mkrefany<T>()
        => Mkrefany(typeof(T));

    public E Refanytype() => Emit(OpCodes.Refanytype);

    public E Refanyval(Type type) => Emit(OpCodes.Refanyval, type);

    public E Refanyval<T>()
        => Refanyval(typeof(T));

    public E Sizeof(Type type) => Emit(OpCodes.Sizeof, type);

    public E Sizeof<T>()
        where T : struct
        => Sizeof(typeof(T));

#endregion

#endregion

#region Custom Helpers

    /// <summary>
    /// Pushes a <typeparamref name="T"/> value onto the stream using the appropriate operations
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public E PushValue<T>(T? value)
    {
        return value switch
        {
            null => Ldnull(),
            bool boolean => boolean ? Ldc_I4_1() : Ldc_I4_0(),
            sbyte i8 => Ldc_I4_S(i8),
            byte u8 => Ldc_I4(u8),
            short i16 => Ldc_I4(i16),
            ushort u16 => Ldc_I4(u16),
            int i32 => Ldc_I4(i32),
            uint u32 => Ldc_I8(u32),
            long i64 => Ldc_I8(i64),
            ulong u64 => Ldc_I8((long)u64).Conv_U8(),
            float f32 => Ldc_R4(f32),
            double f64 => Ldc_R8(f64),
            string str => Ldstr(str),
            Type type => Ldtoken(type).Call(EmissionHelper.Type_GetTypeFromHandle_Method),
            MethodInfo method => Ldtoken(method).Call(EmissionHelper.Method_GetMethodFromHandle_Method),
            ILLocal local => Ldloc(local),
            _ => throw new NotImplementedException(),
        };
    }

    public E PushDefault(Type type)
    {
        if (type.IsValueType)
        {
            // we have to use a local
            return DeclareLocal(type, out var temp)
                .Ldloca(temp)
                .Initobj(type)
                .Ldloc(temp);
        }
        else
        {
            // defalt is null
            return Ldnull();
        }
    }

    public E PushDefault<T>() => PushDefault(typeof(T));

    public E PushDefaultAddr(Type type)
    {
        if (type.IsValueType)
        {
            // we have to use a local
            return DeclareLocal(type, out var temp)
                .Ldloca(temp)
                .Initobj(type)
                .Ldloca(temp);
        }
        else
        {
            // defalt is null
            return Ldnulla();
        }
    }
    
    public E PushDefaultAddr<T>() => PushDefaultAddr(typeof(T));
    
    /// <summary>
    /// Loads a <c>null</c> reference onto the stack
    /// </summary>
    public E Ldnulla() => Ldc_I4_0().Conv_U();
    

    public E MarkLabel(out ILLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null)
    {
        return DefineLabel(out label, labelName)
            .MarkLabel(label);
    }
    

    public ITryCatchFinally<E> Try(Action<E, ILLabel> tryBlock)
    {
        return new TryCatchFinally<E>(_emitter)
            .Try(tryBlock);
    }

    public E BoxIfNeeded(Type type)
    {
        Throw.IfNull(type);
        if (type.IsValueType)
        {
            return Box(type);
        }
        else
        {
            // do nothing
            return _emitter;
        }
    }
#endregion
}