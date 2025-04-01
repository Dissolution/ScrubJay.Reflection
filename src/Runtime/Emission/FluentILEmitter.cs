using ScrubJay.Fluent;
using ScrubJay.Reflection.Naming;
using ScrubJay.Reflection.Runtime.Emission.Instructions;

namespace ScrubJay.Reflection.Runtime.Emission;

public sealed class FluentILEmitter : FluentILEmitter<FluentILEmitter>
{
    public FluentILEmitter(ILGenerator ilGenerator)
        : base(ilGenerator)
    {

    }
}

public abstract class FluentILEmitter<TSelf> : FluentBuilder<TSelf>,
    IFluentILEmitter<TSelf>,
    IFluentOpCodeEmitter<TSelf>,
    IFluentILGeneratorEmitter<TSelf>,
    IFluentOperationEmitter<TSelf>,
    IFluentDirectOperationEmitter<TSelf>
    where TSelf : FluentILEmitter<TSelf>
{
    private readonly ILGenerator _ilGenerator;

    private readonly InstructionStream<Instruction> _instructions = [];
    private readonly Dictionary<EmitterLabel, Label> _labels = [];
    private readonly Dictionary<EmitterLocal, LocalBuilder> _locals = [];

    public IReadOnlyCollection<EmitterLabel> Labels => _labels.Keys;
    public IReadOnlyCollection<EmitterLocal> Locals => _locals.Keys;

    protected FluentILEmitter(ILGenerator ilGenerator) : base()
    {
        _ilGenerator = ilGenerator;
    }

    protected Result<Label> ValidateLabel(
        EmitterLabel? emitterLabel,
        [CallerArgumentExpression(nameof(emitterLabel))]
        string? emitterLabelName = null)
    {
        if (emitterLabel is null)
            return new ArgumentNullException(emitterLabelName);
        if (!_labels.TryGetValue(emitterLabel, out var label))
            return new ArgumentException($"EmitterLabel '{emitterLabel}' does not belong to this Emitter", emitterLabelName);
        return label;
    }

    protected Result<LocalBuilder> ValidateLocal(
        EmitterLocal? emitterLocal,
        [CallerArgumentExpression(nameof(emitterLocal))]
        string? emitterLocalName = null)
    {
        if (emitterLocal is null)
            return new ArgumentNullException(emitterLocalName);
        if (!_locals.TryGetValue(emitterLocal, out var local))
            return new ArgumentException($"EmitterLocal '{emitterLocal}' does not belong to this Emitter", emitterLocalName);
        return local;
    }

    protected string? GetVariableName(string? name) => GetVariableName(name.AsSpan());
    protected string? GetVariableName(ReadOnlySpan<char> name)
    {
        name = name.Trim();
        if (name.Length == 0)
            return null;
        var si = name.LastIndexOf(' ');
        if (si >= 0)
        {
            name = name.Slice(si + 1);
        }
        return name.AsString();
    }

#region IFluentOpCodeEmitter
    protected OpCodeInstruction AddOpInstr(OpCode opCode)
    {
        var offset = _ilGenerator.ILOffset;
        var instr = new OpCodeInstruction(opCode)
        {
            Offset = offset,
        };
        _instructions.Add(instr);
        return instr;
    }

    protected OpCodeInstruction AddOpInstr(OpCode opCode, object arg)
    {
        Debug.Assert(arg is not null);
        var offset = _ilGenerator.ILOffset;
        var instr = new OpCodeInstruction(opCode)
        {
            Offset = offset,
            Operand = Some<object>(arg!),
        };
        _instructions.Add(instr);
        return instr;
    }


    public TSelf Emit(OpCode opCode)
    {
        AddOpInstr(opCode);
        _ilGenerator.Emit(opCode);
        return _builder;
    }

    public TSelf Emit(OpCode opCode, byte u8)
    {
        AddOpInstr(opCode, u8);
        _ilGenerator.Emit(opCode, u8);
        return _builder;
    }

    public TSelf Emit(OpCode opCode, sbyte i8)
    {
        AddOpInstr(opCode, i8);
        _ilGenerator.Emit(opCode, i8);
        return _builder;
    }

    public TSelf Emit(OpCode opCode, short i16)
    {
        AddOpInstr(opCode, i16);
        _ilGenerator.Emit(opCode, i16);
        return _builder;
    }

    public TSelf Emit(OpCode opCode, int i32)
    {
        AddOpInstr(opCode, i32);
        _ilGenerator.Emit(opCode, i32);
        return _builder;
    }

    public TSelf Emit(OpCode opCode, long i64)
    {
        AddOpInstr(opCode, i64);
        _ilGenerator.Emit(opCode, i64);
        return _builder;
    }

    public TSelf Emit(OpCode opCode, float f32)
    {
        AddOpInstr(opCode, f32);
        _ilGenerator.Emit(opCode, f32);
        return _builder;
    }

    public TSelf Emit(OpCode opCode, double i8)
    {
        AddOpInstr(opCode, i8);
        _ilGenerator.Emit(opCode, i8);
        return _builder;
    }

    public TSelf Emit(OpCode opCode, string str)
    {
        Throw.IfEmpty(str);
        AddOpInstr(opCode, str);
        _ilGenerator.Emit(opCode, str);
        return _builder;
    }

    public TSelf Emit(OpCode opCode, EmitterLabel label)
    {
        var lbl = ValidateLabel(label).OkOrThrow();
        AddOpInstr(opCode, label);
        _ilGenerator.Emit(opCode, lbl);
        return _builder;
    }

    public TSelf Emit(OpCode opCode, params EmitterLabel[] labels)
    {
        Throw.IfEmpty(labels);
        AddOpInstr(opCode, labels);
        int count = labels.Length;
        Label[] lbls = new Label[count];
        for (int i = 0; i < count; i++)
        {
            var label = ValidateLabel(labels[i], nameof(labels)).OkOrThrow();
            lbls[i] = label;
        }
        _ilGenerator.Emit(opCode, lbls);
        return _builder;
    }

    public TSelf Emit(OpCode opCode, EmitterLocal local)
    {
        var lcl = ValidateLocal(local).OkOrThrow();
        AddOpInstr(opCode, local);
        _ilGenerator.Emit(opCode, lcl);
        return _builder;
    }

    public TSelf Emit(OpCode opCode, FieldInfo field)
    {
        Throw.IfNull(field);
        AddOpInstr(opCode, field);
        _ilGenerator.Emit(opCode, field);
        return _builder;
    }

    public TSelf Emit(OpCode opCode, ConstructorInfo ctor)
    {
        Throw.IfNull(ctor);
        AddOpInstr(opCode, ctor);
        _ilGenerator.Emit(opCode, ctor);
        return _builder;
    }

    public TSelf Emit(OpCode opCode, MethodInfo method)
    {
        Throw.IfNull(method);
        AddOpInstr(opCode, method);
        _ilGenerator.Emit(opCode, method);
        return _builder;
    }

    public TSelf Emit(OpCode opCode, Type type)
    {
        Throw.IfNull(type);
        AddOpInstr(opCode, type);
        _ilGenerator.Emit(opCode, type);
        return _builder;
    }

    public TSelf Emit(OpCode opCode, SignatureHelper signature)
    {
        Throw.IfNull(signature);
        AddOpInstr(opCode, signature);
        _ilGenerator.Emit(opCode, signature);
        return _builder;
    }
#endregion

#region IFluentILGeneratorEmittier
    protected ILGeneratorInstruction AddGenInstr(ILGeneratorMethod generatorMethod)
    {
        ILGeneratorInstruction instr = new(generatorMethod)
        {
            Offset = _ilGenerator.ILOffset,
        };
        _instructions.Add(instr);
        return instr;
    }

    protected ILGeneratorInstruction AddGenInstr(ILGeneratorMethod generatorMethod, object? arg)
    {
        ILGeneratorInstruction instr = new(generatorMethod, arg)
        {
            Offset = _ilGenerator.ILOffset,
        };
        _instructions.Add(instr);
        return instr;
    }

    protected ILGeneratorInstruction AddGenInstr(ILGeneratorMethod generatorMethod, params object?[] args)
    {
        ILGeneratorInstruction instr = new(generatorMethod, args)
        {
            Offset = _ilGenerator.ILOffset,
        };
        _instructions.Add(instr);
        return instr;
    }



    public TSelf BeginExceptionBlock(out EmitterLabel label,
        [CallerArgumentExpression(nameof(label))]
        string? labelName = null)
    {
        var instr = AddGenInstr(ILGeneratorMethod.BeginExceptionBlock);
        Label lbl = _ilGenerator.BeginExceptionBlock();
        label = new EmitterLabel(lbl, GetVariableName(labelName));
        _labels.Add(label, lbl);
        instr.Parameters = label;
        return _builder;
    }

    public TSelf BeginCatchBlock(Type exceptionType)
    {
        Throw.IfNull(exceptionType);
        if (!exceptionType.Implements<Exception>())
            throw new ArgumentException($"Exception Type '{MemberNames.NameOf(exceptionType)}' is not an Exception", nameof(exceptionType));
        AddGenInstr(ILGeneratorMethod.BeginCatchBlock, exceptionType);
        _ilGenerator.BeginCatchBlock(exceptionType);
        return _builder;
    }

    public TSelf BeginCatchBlock<TException>()
        where TException : Exception
        => BeginCatchBlock(typeof(TException));

    public TSelf BeginFinallyBlock()
    {
        AddGenInstr(ILGeneratorMethod.BeginFinallyBlock);
        _ilGenerator.BeginFinallyBlock();
        return _builder;
    }

    public TSelf BeginExceptFilterBlock()
    {
        AddGenInstr(ILGeneratorMethod.BeginExceptFilterBlock);
        _ilGenerator.BeginExceptFilterBlock();
        return _builder;
    }

    public TSelf BeginFaultBlock()
    {
        AddGenInstr(ILGeneratorMethod.BeginFaultBlock);
        _ilGenerator.BeginFaultBlock();
        return _builder;
    }

    public TSelf EndExceptionBlock()
    {
        AddGenInstr(ILGeneratorMethod.EndExceptionBlock);
        _ilGenerator.EndExceptionBlock();
        return _builder;
    }

    public TSelf BeginScope()
    {
        AddGenInstr(ILGeneratorMethod.BeginScope);
        _ilGenerator.BeginScope();
        return _builder;
    }

    public TSelf EndScope()
    {
        AddGenInstr(ILGeneratorMethod.EndScope);
        _ilGenerator.EndScope();
        return _builder;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="namespace"></param>
    /// <returns></returns>
    /// <remarks>
    /// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/namespaces#143-namespace-declarations
    /// </remarks>
    public TSelf UsingNamespace(string @namespace)
    {
        Throw.IfEmpty(@namespace);
        if (!NameHelper.IsValidNamespace(@namespace))
            throw new ArgumentException($"Namespace '{@namespace}' is not valid", nameof(@namespace));
        AddGenInstr(ILGeneratorMethod.UsingNamespace, @namespace);
        _ilGenerator.UsingNamespace(@namespace);
        return _builder;
    }

    public TSelf DeclareLocal(Type localType, out EmitterLocal local,
        [CallerArgumentExpression(nameof(local))]
        string? localName = null)
    {
        var localBuilder = _ilGenerator.DeclareLocal(localType);
        local = new EmitterLocal(localBuilder, GetVariableName(localName));
        _locals.Add(local, localBuilder);
        return _builder;
    }

    public TSelf DeclareLocal<T>(out EmitterLocal local,
        [CallerArgumentExpression(nameof(local))]
        string? localName = null)
        => DeclareLocal(typeof(T), out local, localName);

    public TSelf DeclareLocal(Type localType, bool pinned, out EmitterLocal local,
        [CallerArgumentExpression(nameof(local))]
        string? localName = null)
    {
        var localBuilder = _ilGenerator.DeclareLocal(localType, pinned);
        local = new EmitterLocal(localBuilder, GetVariableName(localName));
        _locals.Add(local, localBuilder);
        return _builder;
    }

    public TSelf DeclareLocal<T>(bool pinned, out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null)
        => DeclareLocal(typeof(T), pinned, out local, localName);

    public TSelf DefineLabel(out EmitterLabel label,
        [CallerArgumentExpression(nameof(label))]
        string? labelName = null)
    {
        var lbl = _ilGenerator.DefineLabel();
        label = new EmitterLabel(lbl, GetVariableName(labelName));
        _labels.Add(label, lbl);
        return _builder;
    }

    public TSelf MarkLabel(EmitterLabel label)
    {
        var lbl = ValidateLabel(label).OkOrThrow();
        AddGenInstr(ILGeneratorMethod.MarkLabel, label);
        _ilGenerator.MarkLabel(lbl);
        return _builder;
    }

    public TSelf EmitCall(MethodInfo methodInfo, Type[]? optionalParameterTypes = null)
    {
        Throw.IfNull(methodInfo);
        AddGenInstr(ILGeneratorMethod.CallVarargs, methodInfo, optionalParameterTypes);
        _ilGenerator.EmitCall(methodInfo.GetCallOpCode(), methodInfo, optionalParameterTypes);
        return _builder;
    }

    public TSelf EmitCalli(
        CallingConventions callingConvention,
        Type? returnType,
        Type[]? parameterTypes,
        Type[]? optionalParameterTypes = null)
    {
        AddGenInstr(ILGeneratorMethod.CallManaged, returnType, parameterTypes, optionalParameterTypes);
        _ilGenerator.EmitCalli(
            OpCodes.Calli,
            callingConvention, returnType, parameterTypes, optionalParameterTypes);
        return _builder;
    }

    public TSelf EmitCalli(CallingConvention unmanagedCallConv, Type? returnType, Type[]? parameterTypes)
    {
        AddGenInstr(ILGeneratorMethod.CallUnmanaged, returnType, parameterTypes);
#if NETSTANDARD2_0
        throw new NotSupportedException();
#else
        _ilGenerator.EmitCalli(
            OpCodes.Calli,
            unmanagedCallConv, returnType, parameterTypes);
        return _builder;
#endif
    }
#endregion

    public override string ToString()
    {
        using var text = new TextBuilder();
        text.Delimit(static tb => tb.NewLine(), _instructions, static (tb, instr) => tb.Append(instr));
        return text.ToString();
    }

#region IFluentOperationEmitter
    public TSelf Endfilter() => Emit(OpCodes.Endfilter);

    public TSelf Endfinally() => Emit(OpCodes.Endfinally);

    public TSelf Arglist() => Emit(OpCodes.Arglist);

    public TSelf Switch(params EmitterLabel[] labels) => Emit(OpCodes.Switch, labels);

    public TSelf Constrained(Type type) => Emit(OpCodes.Constrained, type);

    public TSelf Constrained<T>()
        => Constrained(typeof(T));

    public TSelf Ldftn(MethodInfo method) => Emit(OpCodes.Ldftn, method);

    public TSelf Ldvirtftn(MethodInfo method) => Emit(OpCodes.Ldvirtftn, method);

    public TSelf Tailcall() => Emit(OpCodes.Tailcall);

    public TSelf Break() => Emit(OpCodes.Break);

    public TSelf Nop() => Emit(OpCodes.Nop);

    public TSelf Ckfinite() => Emit(OpCodes.Ckfinite);

    public TSelf Rethrow() => Emit(OpCodes.Rethrow);

    TSelf IFluentOperationEmitter<TSelf>.Throw() => Emit(OpCodes.Throw);

    public TSelf And() => Emit(OpCodes.And);

    public TSelf Neg() => Emit(OpCodes.Neg);

    public TSelf Not() => Emit(OpCodes.Not);

    public TSelf Or() => Emit(OpCodes.Or);

    public TSelf Shl() => Emit(OpCodes.Shl);

    public TSelf Xor() => Emit(OpCodes.Xor);

    public TSelf Jmp(MethodInfo method) => Emit(OpCodes.Jmp, method);

    public TSelf Ret() => Emit(OpCodes.Ret);

    public TSelf Box(Type type) => Emit(OpCodes.Box, type);

    public TSelf Box<T>()
        => Box(typeof(T));

    public TSelf Unbox(Type type) => Emit(OpCodes.Unbox, type);

    public TSelf Unbox<T>()
        => Unbox(typeof(T));

    public TSelf Unbox_Any(Type type) => Emit(OpCodes.Unbox_Any, type);

    public TSelf Unbox_Any<T>()
        => Unbox_Any(typeof(T));

    public TSelf Castclass(Type type) => Emit(OpCodes.Castclass, type);

    public TSelf Castclass<T>() where T : class
        => Castclass(typeof(T));

    public TSelf Isinst(Type type) => Emit(OpCodes.Isinst, type);

    public TSelf Isinst<T>()
        => Isinst(typeof(T));

    public TSelf Ceq() => Emit(OpCodes.Ceq);

    public TSelf Cpblk() => Emit(OpCodes.Cpblk);

    public TSelf Initblk() => Emit(OpCodes.Initblk);

    public TSelf Localloc() => Emit(OpCodes.Localloc);

    public TSelf Cpobj(Type type) => Emit(OpCodes.Cpobj, type);

    public TSelf Cpobj<T>() where T : struct
        => Cpobj(typeof(T));

    public TSelf Dup() => Emit(OpCodes.Dup);

    public TSelf Initobj(Type type) => Emit(OpCodes.Initobj, type);

    public TSelf Initobj<T>() where T : struct
        => Initobj(typeof(T));

    public TSelf Newobj(ConstructorInfo ctor) => Emit(OpCodes.Newobj, ctor);

    public TSelf Pop() => Emit(OpCodes.Pop);

    public TSelf Ldnull() => Emit(OpCodes.Ldnull);

    public TSelf Ldstr(string str) => Emit(OpCodes.Ldstr, str);

    public TSelf Ldtoken(Type type) => Emit(OpCodes.Ldtoken, type);

    public TSelf Ldtoken(FieldInfo field) => Emit(OpCodes.Ldtoken, field);

    public TSelf Ldtoken(MethodInfo method) => Emit(OpCodes.Ldtoken, method);

    public TSelf Ldlen() => Emit(OpCodes.Ldlen);

    public TSelf Newarr(Type type) => Emit(OpCodes.Newarr, type);

    public TSelf Newarr<T>()
        => Newarr(typeof(T));

    public TSelf Readonly() => Emit(OpCodes.Readonly);

    public TSelf Unaligned(int alignment)
    {
        if (alignment is not (1 or 2 or 4))
            throw new ArgumentOutOfRangeException(nameof(alignment), alignment, "Alignment must be 1, 2, or 4");
        return Emit(OpCodes.Unaligned, alignment);
    }

    public TSelf Volatile() => Emit(OpCodes.Volatile);

    public TSelf Mkrefany(Type type) => Emit(OpCodes.Mkrefany, type);

    public TSelf Mkrefany<T>()
        => Mkrefany(typeof(T));

    public TSelf Refanytype() => Emit(OpCodes.Refanytype);

    public TSelf Refanyval(Type type) => Emit(OpCodes.Refanyval, type);

    public TSelf Refanyval<T>()
        => Refanyval(typeof(T));

    public TSelf Sizeof(Type type) => Emit(OpCodes.Sizeof, type);

    public TSelf Sizeof<T>()
        where T : unmanaged
        => Sizeof(typeof(T));
#endregion

#region Direct
#region Arguments
#region Load Argument
    public TSelf Ldarg_0() => Emit(OpCodes.Ldarg_0);

    public TSelf Ldarg_1() => Emit(OpCodes.Ldarg_1);

    public TSelf Ldarg_2() => Emit(OpCodes.Ldarg_2);

    public TSelf Ldarg_3() => Emit(OpCodes.Ldarg_3);

    public TSelf Ldarg_S(byte index)
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

    public TSelf Ldarg(ushort index)
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

    public TSelf Ldarga(ushort index)
    {
        if (index <= byte.MaxValue)
            return Emit(OpCodes.Ldarga_S, (byte)index);
        return Emit(OpCodes.Ldarga, index);
    }

    public TSelf Ldarga_S(byte index) => Emit(OpCodes.Ldarga_S, index);
#endregion

#region Store in Argument
    public TSelf Starg(ushort index)
    {
        if (index <= byte.MaxValue)
            return Emit(OpCodes.Starg_S, (byte)index);
        return Emit(OpCodes.Starg, index);
    }

    public TSelf Starg_S(byte index) => Emit(OpCodes.Starg_S, index);
#endregion
#endregion
#region Locals
#region Load Local
    public TSelf Ldloc_0() => Emit(OpCodes.Ldloc_0);

    public TSelf Ldloc_1() => Emit(OpCodes.Ldloc_1);

    public TSelf Ldloc_2() => Emit(OpCodes.Ldloc_2);

    public TSelf Ldloc_3() => Emit(OpCodes.Ldloc_3);

    public TSelf Ldloc_S(byte index) => Emit(OpCodes.Ldloc_S, index);

    public TSelf Ldloc(ushort index)
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

    public TSelf Ldloc(EmitterLocal local)
    {
        ValidateLocal(local).ThrowIfError();
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

    public TSelf Ldloc_S(EmitterLocal local) => Ldloc(local);

    public TSelf Ldloca(EmitterLocal local)
    {
        ValidateLocal(local).ThrowIfError();
        if (local.IsShortForm)
            return Emit(OpCodes.Ldloca_S, local);
        return Emit(OpCodes.Ldloca, local);
    }

    public TSelf Ldloca_S(EmitterLocal local) => Ldloca(local);
#endregion
#region Store Local
    public TSelf Stloc_0() => Emit(OpCodes.Stloc_0);
    public TSelf Stloc_1() => Emit(OpCodes.Stloc_1);
    public TSelf Stloc_2() => Emit(OpCodes.Stloc_2);
    public TSelf Stloc_3() => Emit(OpCodes.Stloc_3);

    public TSelf Stloc_S(byte index)
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

    public TSelf Stloc(ushort index)
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



    public TSelf Stloc(EmitterLocal local)
    {
        ValidateLocal(local).ThrowIfError();
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

    public TSelf Stloc_S(EmitterLocal local) => Stloc(local);
#endregion
#endregion

    public TSelf Call(MethodInfo method)
    {
        var callOpCode = method.GetCallOpCode();
        return Emit(callOpCode, method);
    }

    public TSelf Callvirt(MethodInfo method) => Call(method);

    public TSelf Add() => Emit(OpCodes.Add);

    public TSelf Add_Ovf() => Emit(OpCodes.Add_Ovf);

    public TSelf Add_Ovf_Un() => Emit(OpCodes.Add_Ovf_Un);

    public TSelf Div() => Emit(OpCodes.Div);

    public TSelf Div_Un() => Emit(OpCodes.Div_Un);

    public TSelf Mul() => Emit(OpCodes.Mul);

    public TSelf Mul_Ovf() => Emit(OpCodes.Mul_Ovf);

    public TSelf Mul_Ovf_Un() => Emit(OpCodes.Mul_Ovf_Un);

    public TSelf Rem() => Emit(OpCodes.Rem);

    public TSelf Rem_Un() => Emit(OpCodes.Rem_Un);

    public TSelf Sub() => Emit(OpCodes.Sub);

    public TSelf Sub_Ovf() => Emit(OpCodes.Sub_Ovf);

    public TSelf Sub_Ovf_Un() => Emit(OpCodes.Sub_Ovf_Un);

    public TSelf Shr() => Emit(OpCodes.Shr);

    public TSelf Shr_Un() => Emit(OpCodes.Shr_Un);

#region Break / Leave
    public TSelf Br(EmitterLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Br_S, label);
        return Emit(OpCodes.Br, label);
    }

    public TSelf Br_S(EmitterLabel label) => Br(label);

    public TSelf Leave(EmitterLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Leave_S, label);
        return Emit(OpCodes.Leave, label);
    }

    public TSelf Leave_S(EmitterLabel label) => Leave(label);

    public TSelf Brtrue(EmitterLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Brtrue_S, label);
        return Emit(OpCodes.Brtrue, label);
    }

    public TSelf Brtrue_S(EmitterLabel label) => Brtrue(label);

    public TSelf Brfalse(EmitterLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Brfalse_S, label);
        return Emit(OpCodes.Brfalse, label);
    }

    public TSelf Brfalse_S(EmitterLabel label) => Brfalse(label);

    public TSelf Beq(EmitterLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Beq_S, label);
        return Emit(OpCodes.Beq, label);
    }

    public TSelf Beq_S(EmitterLabel label) => Beq(label);

    public TSelf Bne_Un(EmitterLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Bne_Un_S, label);
        return Emit(OpCodes.Bne_Un, label);
    }

    public TSelf Bne_Un_S(EmitterLabel label) => Bne_Un(label);

    public TSelf Bge(EmitterLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Bge_S, label);
        return Emit(OpCodes.Bge, label);
    }

    public TSelf Bge_S(EmitterLabel label) => Bge(label);

    public TSelf Bge_Un(EmitterLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Bge_Un_S, label);
        return Emit(OpCodes.Bge_Un, label);
    }

    public TSelf Bge_Un_S(EmitterLabel label) => Bge_Un(label);

    public TSelf Bgt(EmitterLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Bgt_S, label);
        return Emit(OpCodes.Bgt, label);
    }

    public TSelf Bgt_S(EmitterLabel label) => Bgt(label);

    public TSelf Bgt_Un(EmitterLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Bgt_Un_S, label);
        return Emit(OpCodes.Bgt_Un, label);
    }

    public TSelf Bgt_Un_S(EmitterLabel label) => Bgt_Un(label);

    public TSelf Ble(EmitterLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Ble_S, label);
        return Emit(OpCodes.Ble, label);
    }

    public TSelf Ble_S(EmitterLabel label) => Ble(label);

    public TSelf Ble_Un(EmitterLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Ble_Un_S, label);
        return Emit(OpCodes.Ble_Un, label);
    }

    public TSelf Ble_Un_S(EmitterLabel label) => Ble_Un(label);

    public TSelf Blt(EmitterLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Blt_S, label);
        return Emit(OpCodes.Blt, label);
    }

    public TSelf Blt_S(EmitterLabel label) => Blt(label);

    public TSelf Blt_Un(EmitterLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Blt_Un_S, label);
        return Emit(OpCodes.Blt_Un, label);
    }

    public TSelf Blt_Un_S(EmitterLabel label) => Blt_Un(label);
#endregion

#region Convert
    public TSelf Conv_I() => Emit(OpCodes.Conv_I);

    public TSelf Conv_Ovf_I() => Emit(OpCodes.Conv_Ovf_I);

    public TSelf Conv_Ovf_I_Un() => Emit(OpCodes.Conv_Ovf_I_Un);

    public TSelf Conv_I1() => Emit(OpCodes.Conv_I1);

    public TSelf Conv_Ovf_I1() => Emit(OpCodes.Conv_Ovf_I1);

    public TSelf Conv_Ovf_I1_Un() => Emit(OpCodes.Conv_Ovf_I1_Un);

    public TSelf Conv_I2() => Emit(OpCodes.Conv_I2);

    public TSelf Conv_Ovf_I2() => Emit(OpCodes.Conv_Ovf_I2);

    public TSelf Conv_Ovf_I2_Un() => Emit(OpCodes.Conv_Ovf_I2_Un);

    public TSelf Conv_I4() => Emit(OpCodes.Conv_I4);

    public TSelf Conv_Ovf_I4() => Emit(OpCodes.Conv_Ovf_I4);

    public TSelf Conv_Ovf_I4_Un() => Emit(OpCodes.Conv_Ovf_I4_Un);

    public TSelf Conv_I8() => Emit(OpCodes.Conv_I8);

    public TSelf Conv_Ovf_I8() => Emit(OpCodes.Conv_Ovf_I8);

    public TSelf Conv_Ovf_I8_Un() => Emit(OpCodes.Conv_Ovf_I8_Un);

    public TSelf Conv_U() => Emit(OpCodes.Conv_U);

    public TSelf Conv_Ovf_U() => Emit(OpCodes.Conv_Ovf_U);

    public TSelf Conv_Ovf_U_Un() => Emit(OpCodes.Conv_Ovf_U_Un);

    public TSelf Conv_U1() => Emit(OpCodes.Conv_U1);

    public TSelf Conv_Ovf_U1() => Emit(OpCodes.Conv_Ovf_U1);

    public TSelf Conv_Ovf_U1_Un() => Emit(OpCodes.Conv_Ovf_U1_Un);

    public TSelf Conv_U2() => Emit(OpCodes.Conv_U2);

    public TSelf Conv_Ovf_U2() => Emit(OpCodes.Conv_Ovf_U2);

    public TSelf Conv_Ovf_U2_Un() => Emit(OpCodes.Conv_Ovf_U2_Un);

    public TSelf Conv_U4() => Emit(OpCodes.Conv_U4);

    public TSelf Conv_Ovf_U4() => Emit(OpCodes.Conv_Ovf_U4);

    public TSelf Conv_Ovf_U4_Un() => Emit(OpCodes.Conv_Ovf_U4_Un);

    public TSelf Conv_U8() => Emit(OpCodes.Conv_U8);

    public TSelf Conv_Ovf_U8() => Emit(OpCodes.Conv_Ovf_U8);

    public TSelf Conv_Ovf_U8_Un() => Emit(OpCodes.Conv_Ovf_U8_Un);

    public TSelf Conv_R_Un() => Emit(OpCodes.Conv_R_Un);

    public TSelf Conv_R4() => Emit(OpCodes.Conv_R4);

    public TSelf Conv_R8() => Emit(OpCodes.Conv_R8);
#endregion

#region Compare
    public TSelf Cgt()
    {
        throw new NotImplementedException();
    }

    public TSelf Cgt_Un()
    {
        throw new NotImplementedException();
    }

    public TSelf Clt()
    {
        throw new NotImplementedException();
    }

    public TSelf Clt_Un()
    {
        throw new NotImplementedException();
    }
#endregion

#region Load Constant
    public TSelf Ldc_I4_M1() => Emit(OpCodes.Ldc_I4_M1);

    public TSelf Ldc_I4_0() => Emit(OpCodes.Ldc_I4_0);

    public TSelf Ldc_I4_1() => Emit(OpCodes.Ldc_I4_1);

    public TSelf Ldc_I4_2() => Emit(OpCodes.Ldc_I4_2);

    public TSelf Ldc_I4_3() => Emit(OpCodes.Ldc_I4_3);

    public TSelf Ldc_I4_4() => Emit(OpCodes.Ldc_I4_4);

    public TSelf Ldc_I4_5() => Emit(OpCodes.Ldc_I4_5);

    public TSelf Ldc_I4_6() => Emit(OpCodes.Ldc_I4_6);

    public TSelf Ldc_I4_7() => Emit(OpCodes.Ldc_I4_7);

    public TSelf Ldc_I4_8() => Emit(OpCodes.Ldc_I4_8);

    public TSelf Ldc_I4_S(sbyte value)
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

    public TSelf Ldc_I4(int value)
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

    public TSelf Ldc_I8(long value) => Emit(OpCodes.Ldc_I8, value);

    public TSelf Ldc_R4(float value) => Emit(OpCodes.Ldc_R4, value);

    public TSelf Ldc_R8(double value) => Emit(OpCodes.Ldc_R8, value);
#endregion

#region Array Elements
#region Load array Element
    public TSelf Ldelem_I() => Emit(OpCodes.Ldelem_I);

    public TSelf Ldelem_I1() => Emit(OpCodes.Ldelem_I1);

    public TSelf Ldelem_I2() => Emit(OpCodes.Ldelem_I2);

    public TSelf Ldelem_I4() => Emit(OpCodes.Ldelem_I4);

    public TSelf Ldelem_I8() => Emit(OpCodes.Ldelem_I8);

    public TSelf Ldelem_U1() => Emit(OpCodes.Ldelem_U1);

    public TSelf Ldelem_U2() => Emit(OpCodes.Ldelem_U2);

    public TSelf Ldelem_U4() => Emit(OpCodes.Ldelem_U4);

    public TSelf Ldelem_R4() => Emit(OpCodes.Ldelem_R4);

    public TSelf Ldelem_R8() => Emit(OpCodes.Ldelem_R8);

    public TSelf Ldelem_Ref() => Emit(OpCodes.Ldelem_Ref);

    public TSelf Ldelem(Type type)
    {
        if (type == typeof(nint))
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

    public TSelf Ldelem<T>() => Ldelem(typeof(T));

    public TSelf Ldelema(Type type) => Emit(OpCodes.Ldelema, type);

    public TSelf Ldelema<T>() => Ldelema(typeof(T));
#endregion

#region Store in array Element
    public TSelf Stelem_I() => Emit(OpCodes.Stelem_I);

    public TSelf Stelem_I1() => Emit(OpCodes.Stelem_I1);

    public TSelf Stelem_I2() => Emit(OpCodes.Stelem_I2);

    public TSelf Stelem_I4() => Emit(OpCodes.Stelem_I4);

    public TSelf Stelem_I8() => Emit(OpCodes.Stelem_I8);

    public TSelf Stelem_R4() => Emit(OpCodes.Stelem_R4);

    public TSelf Stelem_R8() => Emit(OpCodes.Stelem_R8);

    public TSelf Stelem_Ref() => Emit(OpCodes.Stelem_Ref);

    public TSelf Stelem(Type type)
    {
        if (type == typeof(nint))
            return Stelem_I();
        if (type == typeof(sbyte))
            return Stelem_I1();
        if (type == typeof(short))
            return Stelem_I2();
        if (type == typeof(int))
            return Stelem_I4();
        if (type == typeof(long))
            return Stelem_I8();
        if (type == typeof(object))
            return Stelem_Ref();
        return Emit(OpCodes.Stelem, type);
    }

    public TSelf Stelem<T>() => Stelem(typeof(T));


#endregion
#endregion

#region Fields
#region Load from Field
    public TSelf Ldfld(FieldInfo field)
    {
        Throw.IfNull(field);
        if (field.IsStatic)
            return Emit(OpCodes.Ldsfld, field);
        return Emit(OpCodes.Ldfld, field);
    }

    public TSelf Ldsfld(FieldInfo field) => Ldfld(field);

    public TSelf Ldflda(FieldInfo field)
    {
        Throw.IfNull(field);
        if (field.IsStatic)
            return Emit(OpCodes.Ldsflda, field);
        return Emit(OpCodes.Ldflda, field);
    }
    public TSelf Ldsflda(FieldInfo field) => Ldflda(field);
#endregion
#region Store in Field
    public TSelf Stfld(FieldInfo field)
    {
        Throw.IfNull(field);
        if (field.IsStatic)
            return Emit(OpCodes.Stsfld, field);
        return Emit(OpCodes.Stfld, field);
    }

    public TSelf Stsfld(FieldInfo field) => Stfld(field);
#endregion
#endregion

    public TSelf Ldobj(Type type)
    {
        throw new NotImplementedException();
    }

    public TSelf Ldobj<T>()
    {
        throw new NotImplementedException();
    }

    public TSelf Ldind_I()
    {
        throw new NotImplementedException();
    }

    public TSelf Ldind_I1()
    {
        throw new NotImplementedException();
    }

    public TSelf Ldind_I2()
    {
        throw new NotImplementedException();
    }

    public TSelf Ldind_I4()
    {
        throw new NotImplementedException();
    }

    public TSelf Ldind_I8()
    {
        throw new NotImplementedException();
    }

    public TSelf Ldind_U1()
    {
        throw new NotImplementedException();
    }

    public TSelf Ldind_U2()
    {
        throw new NotImplementedException();
    }

    public TSelf Ldind_U4()
    {
        throw new NotImplementedException();
    }

    public TSelf Ldind_R4()
    {
        throw new NotImplementedException();
    }

    public TSelf Ldind_R8()
    {
        throw new NotImplementedException();
    }

    public TSelf Ldind_Ref()
    {
        throw new NotImplementedException();
    }

    public TSelf Stobj(Type type)
    {
        throw new NotImplementedException();
    }

    public TSelf Stobj<T>()
    {
        throw new NotImplementedException();
    }

    public TSelf Stind_I()
    {
        throw new NotImplementedException();
    }

    public TSelf Stind_I1()
    {
        throw new NotImplementedException();
    }

    public TSelf Stind_I2()
    {
        throw new NotImplementedException();
    }

    public TSelf Stind_I4()
    {
        throw new NotImplementedException();
    }

    public TSelf Stind_I8() => Emit(OpCodes.Stind_I8);

    public TSelf Stind_R4() => Emit(OpCodes.Stind_R4);

    public TSelf Stind_R8() => Emit(OpCodes.Stind_R8);

    public TSelf Stind_Ref() => Emit(OpCodes.Stind_Ref);
#endregion


}
