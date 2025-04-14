// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

using ScrubJay.Fluent;
using ScrubJay.Reflection.IL.Instructions;
using ScrubJay.Reflection.Naming;

namespace ScrubJay.Reflection.IL.Emission;

public interface IEmitter<S>
    where S : IEmitter<S>
{
    InstructionStream Instructions { get; }
}

public abstract class Emitter<S> : FluentBuilder<S>, IEmitter<S>
    where S : Emitter<S>
{
    public InstructionStream Instructions { get; } = [];
}

public abstract class EmitterBase<S> : Emitter<S>,
    IEmitter<S>,
    IOpCodeEmitter<S>,
    IGenEmitter<S>,
    IOperationEmitter<S>
    where S : EmitterBase<S>
{
    private readonly ILGenerator? _ilGenerator;

    private readonly Dictionary<CILLabel, Label> _labels = [];
    private readonly Dictionary<CILLocal, LocalBuilder> _locals = [];

    public IReadOnlyCollection<CILLabel> Labels => _labels.Keys;

    public IReadOnlyCollection<CILLocal> Locals => _locals.Keys;

    protected EmitterBase(ILGenerator? ilGenerator)
    {
        _ilGenerator = ilGenerator;
    }

    protected Result<Label> ValidateLabel(
        CILLabel CILLabel,
        [CallerArgumentExpression(nameof(CILLabel))]
        string? CILLabelName = null)
    {
        if (!_labels.TryGetValue(CILLabel, out var label))
            return new ArgumentException($"CILLabel '{CILLabel}' does not belong to this Emitter", CILLabelName);
        return label;
    }

    protected Result<LocalBuilder> ValidateLocal(
        CILLocal CILLocal,
        [CallerArgumentExpression(nameof(CILLocal))]
        string? CILLocalName = null)
    {
        if (!_locals.TryGetValue(CILLocal, out var local))
            return new ArgumentException($"CILLocal '{CILLocal}' does not belong to this Emitter", CILLocalName);
        return local;
    }

    protected string? GetVariableName(string? name) => GetVariableName(name.AsSpan());

    protected string? GetVariableName(scoped text name)
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

#region IOpEmitter

    public S Emit(OpCode opCode)
    {
        _ilGenerator?.Emit(opCode);
        Instructions.Add(new OpCodeInstruction(opCode));
        return _builder;
    }

    public S Emit(OpCode opCode, byte u8)
    {
        _ilGenerator?.Emit(opCode, u8);

        Debugger.Break();
        return _builder;
    }

    public S Emit(OpCode opCode, sbyte i8)
    {
        _ilGenerator?.Emit(opCode, i8);
        Debugger.Break();
        return _builder;
    }

    public S Emit(OpCode opCode, short i16)
    {
        _ilGenerator?.Emit(opCode, i16);
        Debugger.Break();
        return _builder;
    }

    public S Emit(OpCode opCode, int i32)
    {
        _ilGenerator?.Emit(opCode, i32);
        Debugger.Break();
        return _builder;
    }

    public S Emit(OpCode opCode, long i64)
    {
        _ilGenerator?.Emit(opCode, i64);
        Debugger.Break();
        return _builder;
    }

    public S Emit(OpCode opCode, float f32)
    {
        _ilGenerator?.Emit(opCode, f32);
        Debugger.Break();
        return _builder;
    }

    public S Emit(OpCode opCode, double i8)
    {
        _ilGenerator?.Emit(opCode, i8);
        Debugger.Break();
        return _builder;
    }

    public S Emit(OpCode opCode, string str)
    {
        Throw.IfNull(str);
        _ilGenerator?.Emit(opCode, str);
        Debugger.Break();
        return _builder;
    }

    public S Emit(OpCode opCode, CILLabel label)
    {
        var lbl = ValidateLabel(label).OkOrThrow();
        _ilGenerator?.Emit(opCode, lbl);
        Debugger.Break();
        return _builder;
    }

    public S Emit(OpCode opCode, params CILLabel[] cilLabels)
    {
        Throw.IfEmpty(cilLabels);

        int count = cilLabels.Length;
        Label[] labels = new Label[count];
        for (int i = 0; i < count; i++)
        {
            var label = ValidateLabel(cilLabels[i], nameof(cilLabels)).OkOrThrow();
            labels[i] = label;
        }

        _ilGenerator?.Emit(opCode, labels);
        Debugger.Break();
        return _builder;
    }

    public S Emit(OpCode opCode, CILLocal local)
    {
        var lcl = ValidateLocal(local).OkOrThrow();
        _ilGenerator?.Emit(opCode, lcl);
        Debugger.Break();
        return _builder;
    }

    public S Emit(OpCode opCode, FieldInfo field)
    {
        Throw.IfNull(field);
        _ilGenerator?.Emit(opCode, field);
        Debugger.Break();
        return _builder;
    }

    public S Emit(OpCode opCode, ConstructorInfo ctor)
    {
        Throw.IfNull(ctor);
        _ilGenerator?.Emit(opCode, ctor);
        Debugger.Break();
        return _builder;
    }

    public S Emit(OpCode opCode, MethodInfo method)
    {
        Throw.IfNull(method);
        _ilGenerator?.Emit(opCode, method);
        Debugger.Break();
        return _builder;
    }

    public S Emit(OpCode opCode, Type type)
    {
        Throw.IfNull(type);
        _ilGenerator?.Emit(opCode, type);
        Debugger.Break();
        return _builder;
    }

    public S Emit(OpCode opCode, SignatureHelper signature)
    {
        Throw.IfNull(signature);
        _ilGenerator?.Emit(opCode, signature);
        Debugger.Break();
        return _builder;
    }

#endregion

#region IGenEmitter

    public S BeginExceptionBlock(out CILLabel label,
        [CallerArgumentExpression(nameof(label))]
        string? labelName = null)
    {
        Label lbl;
        if (_ilGenerator is null)
        {
            lbl = CILLabel.CreateLabel(_labels.Count);
        }
        else
        {
            lbl = _ilGenerator.BeginExceptionBlock();
        }
        label = new CILLabel(lbl, GetVariableName(labelName));
        _labels.Add(label, lbl);
        Instructions.Add(new ILGeneratorBeginExceptionBlockInstruction(label));
        return _builder;
    }

    public S BeginCatchBlock(Type exceptionType)
    {
        Throw.IfNull(exceptionType);
        if (!exceptionType.Implements<Exception>())
            throw new ArgumentException($"Exception Type '{exceptionType.NameOf()}' is not an Exception", nameof(exceptionType));
        _ilGenerator?.BeginCatchBlock(exceptionType);
        Instructions.Add(new ILGeneratorBeginCatchBlockInstruction(exceptionType));
        return _builder;
    }

    public S BeginCatchBlock<TException>()
        where TException : Exception
        => BeginCatchBlock(typeof(TException));

    public S BeginFinallyBlock()
    {
        _ilGenerator?.BeginFinallyBlock();
        Instructions.Add(new ILGeneratorInstruction(ILGeneratorMethod.BeginFinallyBlock));
        return _builder;
    }

    public S BeginExceptFilterBlock()
    {
        _ilGenerator?.BeginExceptFilterBlock();
        Instructions.Add(new ILGeneratorInstruction(ILGeneratorMethod.BeginExceptFilterBlock));
        return _builder;
    }

    public S BeginFaultBlock()
    {
        _ilGenerator?.BeginFaultBlock();
        Instructions.Add(new ILGeneratorInstruction(ILGeneratorMethod.BeginFaultBlock));
        return _builder;
    }

    public S EndExceptionBlock()
    {
        _ilGenerator?.EndExceptionBlock();
        Instructions.Add(new ILGeneratorInstruction(ILGeneratorMethod.EndExceptionBlock));
        return _builder;
    }

    public S BeginScope()
    {
        _ilGenerator?.BeginScope();
        Instructions.Add(new ILGeneratorInstruction(ILGeneratorMethod.BeginScope));
        return _builder;
    }

    public S EndScope()
    {
        _ilGenerator?.EndScope();
        Instructions.Add(new ILGeneratorInstruction(ILGeneratorMethod.EndScope));
        return _builder;
    }

    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/namespaces#143-namespace-declarations"/>
    public S UsingNamespace(string @namespace)
    {
        Throw.IfEmpty(@namespace);
        if (!CodeHelper.IsValidNamespace(@namespace))
            throw new ArgumentException($"Namespace '{@namespace}' is not valid", nameof(@namespace));
        _ilGenerator?.UsingNamespace(@namespace);
        Instructions.Add(new ILGeneratorUsingNamespaceInstruction(@namespace));
        return _builder;
    }

    public S DeclareLocal(Type localType, out CILLocal local,
        [CallerArgumentExpression(nameof(local))]
        string? localName = null)
    {
        var localBuilder = _ilGenerator!.DeclareLocal(localType);
        local = new CILLocal(localBuilder, GetVariableName(localName));
        _locals.Add(local, localBuilder);
        Instructions.Add(new ILGeneratorDeclareLocalInstruction(local));
        return _builder;
    }

    public S DeclareLocal<T>(out CILLocal local,
        [CallerArgumentExpression(nameof(local))]
        string? localName = null)
        => DeclareLocal(typeof(T), out local, localName);

    public S DeclareLocal(Type localType, bool pinned, out CILLocal local,
        [CallerArgumentExpression(nameof(local))]
        string? localName = null)
    {
        var localBuilder = _ilGenerator!.DeclareLocal(localType, pinned);
        local = new CILLocal(localBuilder, GetVariableName(localName));
        _locals.Add(local, localBuilder);
        Instructions.Add(new ILGeneratorDeclareLocalInstruction(local));
        return _builder;
    }

    public S DeclareLocal<T>(bool pinned, out CILLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null)
        => DeclareLocal(typeof(T), pinned, out local, localName);

    public S DefineLabel(out CILLabel label,
        [CallerArgumentExpression(nameof(label))]
        string? labelName = null)
    {
        Label lbl;
        if (_ilGenerator is null)
        {
            lbl = CILLabel.CreateLabel(_labels.Count);
        }
        else
        {
            lbl = _ilGenerator.DefineLabel();
        }

        label = new CILLabel(lbl, GetVariableName(labelName));
        _labels.Add(label, lbl);
        Instructions.Add(new ILGeneratorDefineLabelInstruction(label));
        return _builder;
    }

    public S MarkLabel(CILLabel label)
    {
        var lbl = ValidateLabel(label).OkOrThrow();
        _ilGenerator?.MarkLabel(lbl);
        Instructions.Add(new ILGeneratorMarkLabelInstruction(label));
        return _builder;
    }

    public S EmitCall(MethodInfo methodInfo, Type[]? optionalParameterTypes = null)
    {
        Throw.IfNull(methodInfo);
        var callOpCode = methodInfo.GetCallOpCode();

        _ilGenerator?.EmitCall(callOpCode, methodInfo, optionalParameterTypes);
        Instructions.Add(new ILGeneratorCallVarargsInstruction(callOpCode, methodInfo, optionalParameterTypes));
        return _builder;
    }

    public S EmitCalli(
        CallingConventions callingConventions,
        Type? returnType,
        Type[]? parameterTypes,
        Type[]? optionalParameterTypes = null)
    {
        _ilGenerator?.EmitCalli(
            OpCodes.Calli,
            callingConventions, returnType, parameterTypes, optionalParameterTypes);
        Instructions.Add(new ILGeneratorCallManagedInstruction(callingConventions, returnType, parameterTypes, optionalParameterTypes));
        return _builder;
    }

#if !NETSTANDARD2_0
    public S EmitCalli(CallingConvention unmanagedCallConv, Type? returnType, Type[]? parameterTypes)
    {
        _ilGenerator?.EmitCalli(
            OpCodes.Calli,
            unmanagedCallConv, returnType, parameterTypes);
        Instructions.Add(new ILGeneratorCallUnmanagedInstruction(unmanagedCallConv, returnType, parameterTypes));
        return _builder;
    }
#endif

#endregion

    public override string ToString()
    {
        using var text = new TextBuilder();
        text.Delimit(static tb => tb.NewLine(), Instructions, static (tb, instr) => tb.Append(instr));
        return text.ToString();
    }

#region IOperationEmitter

#region Math Operators

    public S Add() => Emit(OpCodes.Add);

    public S Add_Ovf() => Emit(OpCodes.Add_Ovf);

    public S Add_Ovf_Un() => Emit(OpCodes.Add_Ovf_Un);

    public S Div() => Emit(OpCodes.Div);

    public S Div_Un() => Emit(OpCodes.Div_Un);

    public S Mul() => Emit(OpCodes.Mul);

    public S Mul_Ovf() => Emit(OpCodes.Mul_Ovf);

    public S Mul_Ovf_Un() => Emit(OpCodes.Mul_Ovf_Un);

    public S Rem() => Emit(OpCodes.Rem);

    public S Rem_Un() => Emit(OpCodes.Rem_Un);

    public S Sub() => Emit(OpCodes.Sub);

    public S Sub_Ovf() => Emit(OpCodes.Sub_Ovf);

    public S Sub_Ovf_Un() => Emit(OpCodes.Sub_Ovf_Un);

#endregion

#region Bitwise Operators

    public S And() => Emit(OpCodes.And);

    public S Neg() => Emit(OpCodes.Neg);

    public S Not() => Emit(OpCodes.Not);

    public S Or() => Emit(OpCodes.Or);

    public S Shl() => Emit(OpCodes.Shl);

    public S Shr() => Emit(OpCodes.Shr);

    public S Shr_Un() => Emit(OpCodes.Shr_Un);

    public S Xor() => Emit(OpCodes.Xor);

#endregion

#region Method related

    public S Arglist() => Emit(OpCodes.Arglist);

    public S Call(MethodInfo method) => Emit(method.GetCallOpCode(), method);

    public S Callvirt(MethodInfo method) => Call(method);

    public S Constrained(Type type) => Emit(OpCodes.Constrained, type);

    public S Constrained<T>() => Constrained(typeof(T));

    public S Ldftn(MethodInfo method) => Emit(OpCodes.Ldftn, method);

    public S Ldvirtftn(MethodInfo method) => Emit(OpCodes.Ldvirtftn, method);

    public S Tailcall() => Emit(OpCodes.Tailcall);

#endregion

#region Branching

#region Comparison

    public S Beq(CILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Beq_S, label);
        return Emit(OpCodes.Beq, label);
    }

    public S Beq_S(CILLabel label) => Beq(label);

    public S Bge(CILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Bge_S, label);
        return Emit(OpCodes.Bge, label);
    }

    public S Bge_S(CILLabel label) => Bge(label);

    public S Bge_Un(CILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Bge_Un_S, label);
        return Emit(OpCodes.Bge_Un, label);
    }

    public S Bge_Un_S(CILLabel label) => Bge_Un(label);

    public S Bgt(CILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Bgt_S, label);
        return Emit(OpCodes.Bgt, label);
    }

    public S Bgt_S(CILLabel label) => Bgt(label);

    public S Bgt_Un(CILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Bgt_Un_S, label);
        return Emit(OpCodes.Bgt_Un, label);
    }

    public S Bgt_Un_S(CILLabel label) => Bgt_Un(label);

    public S Ble(CILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Ble_S, label);
        return Emit(OpCodes.Ble, label);
    }

    public S Ble_S(CILLabel label) => Ble(label);

    public S Ble_Un(CILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Ble_Un_S, label);
        return Emit(OpCodes.Ble_Un, label);
    }

    public S Ble_Un_S(CILLabel label) => Ble_Un(label);

    public S Blt(CILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Blt_S, label);
        return Emit(OpCodes.Blt, label);
    }

    public S Blt_S(CILLabel label) => Blt(label);

    public S Blt_Un(CILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Blt_Un_S, label);
        return Emit(OpCodes.Blt_Un, label);
    }

    public S Blt_Un_S(CILLabel label) => Blt_Un(label);

    public S Bne_Un(CILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Bne_Un_S, label);
        return Emit(OpCodes.Bne_Un, label);
    }

    public S Bne_Un_S(CILLabel label) => Bne_Un(label);

    public S Brfalse(CILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Brfalse_S, label);
        return Emit(OpCodes.Brfalse, label);
    }

    public S Brfalse_S(CILLabel label) => Brfalse(label);

    public S Brtrue(CILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Brtrue_S, label);
        return Emit(OpCodes.Brtrue, label);
    }

    public S Brtrue_S(CILLabel label) => Brtrue(label);

#endregion

#region Unconditional

    public S Br(CILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Br_S, label);
        return Emit(OpCodes.Br, label);
    }

    public S Br_S(CILLabel label) => Br(label);

    public S Leave(CILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Leave_S, label);
        return Emit(OpCodes.Leave, label);
    }

    public S Leave_S(CILLabel label) => Leave(label);

#endregion

    public S Jmp(MethodInfo method) => Emit(OpCodes.Jmp, method);

    public S Ret() => Emit(OpCodes.Ret);

    public S Switch(params CILLabel[] labels) => Emit(OpCodes.Switch, labels);

#endregion

#region Boxing, Unboxing, Casting

    public S Box(Type type) => Emit(OpCodes.Box, type);

    public S Box<T>()
        => Box(typeof(T));

    public S Castclass(Type type) => Emit(OpCodes.Castclass, type);

    public S Castclass<T>() where T : class
        => Castclass(typeof(T));

    public S Isinst(Type type) => Emit(OpCodes.Isinst, type);

    public S Isinst<T>()
        => Isinst(typeof(T));

    public S Unbox(Type type) => Emit(OpCodes.Unbox, type);

    public S Unbox<T>()
        => Unbox(typeof(T));

    public S Unbox_Any(Type type) => Emit(OpCodes.Unbox_Any, type);

    public S Unbox_Any<T>()
        => Unbox_Any(typeof(T));

#endregion

#region Debugging

    public S Break() => Emit(OpCodes.Break);

    public S Nop() => Emit(OpCodes.Nop);

#endregion

#region Comparison

    public S Ceq() => Emit(OpCodes.Ceq);

    public S Cgt() => Emit(OpCodes.Cgt);

    public S Cgt_Un() => Emit(OpCodes.Cgt_Un);

    public S Clt() => Emit(OpCodes.Clt);

    public S Clt_Un() => Emit(OpCodes.Clt_Un);

#endregion

#region Exceptions

    public S Ckfinite() => Emit(OpCodes.Ckfinite);

    public S Endfilter() => Emit(OpCodes.Endfilter);

    public S Endfinally() => Emit(OpCodes.Endfinally);

    public S Rethrow() => Emit(OpCodes.Rethrow);

    S IOperationEmitter<S>.Throw() => Emit(OpCodes.Throw);

#endregion

#region Value Conversion

    public S Conv_I() => Emit(OpCodes.Conv_I);

    public S Conv_Ovf_I() => Emit(OpCodes.Conv_Ovf_I);

    public S Conv_Ovf_I_Un() => Emit(OpCodes.Conv_Ovf_I_Un);

    public S Conv_I1() => Emit(OpCodes.Conv_I1);

    public S Conv_Ovf_I1() => Emit(OpCodes.Conv_Ovf_I1);

    public S Conv_Ovf_I1_Un() => Emit(OpCodes.Conv_Ovf_I1_Un);

    public S Conv_I2() => Emit(OpCodes.Conv_I2);

    public S Conv_Ovf_I2() => Emit(OpCodes.Conv_Ovf_I2);

    public S Conv_Ovf_I2_Un() => Emit(OpCodes.Conv_Ovf_I2_Un);

    public S Conv_I4() => Emit(OpCodes.Conv_I4);

    public S Conv_Ovf_I4() => Emit(OpCodes.Conv_Ovf_I4);

    public S Conv_Ovf_I4_Un() => Emit(OpCodes.Conv_Ovf_I4_Un);

    public S Conv_I8() => Emit(OpCodes.Conv_I8);

    public S Conv_Ovf_I8() => Emit(OpCodes.Conv_Ovf_I8);

    public S Conv_Ovf_I8_Un() => Emit(OpCodes.Conv_Ovf_I8_Un);

    public S Conv_U() => Emit(OpCodes.Conv_U);

    public S Conv_Ovf_U() => Emit(OpCodes.Conv_Ovf_U);

    public S Conv_Ovf_U_Un() => Emit(OpCodes.Conv_Ovf_U_Un);

    public S Conv_U1() => Emit(OpCodes.Conv_U1);

    public S Conv_Ovf_U1() => Emit(OpCodes.Conv_Ovf_U1);

    public S Conv_Ovf_U1_Un() => Emit(OpCodes.Conv_Ovf_U1_Un);

    public S Conv_U2() => Emit(OpCodes.Conv_U2);

    public S Conv_Ovf_U2() => Emit(OpCodes.Conv_Ovf_U2);

    public S Conv_Ovf_U2_Un() => Emit(OpCodes.Conv_Ovf_U2_Un);

    public S Conv_U4() => Emit(OpCodes.Conv_U4);

    public S Conv_Ovf_U4() => Emit(OpCodes.Conv_Ovf_U4);

    public S Conv_Ovf_U4_Un() => Emit(OpCodes.Conv_Ovf_U4_Un);

    public S Conv_U8() => Emit(OpCodes.Conv_U8);

    public S Conv_Ovf_U8() => Emit(OpCodes.Conv_Ovf_U8);

    public S Conv_Ovf_U8_Un() => Emit(OpCodes.Conv_Ovf_U8_Un);

    public S Conv_R_Un() => Emit(OpCodes.Conv_R_Un);

    public S Conv_R4() => Emit(OpCodes.Conv_R4);

    public S Conv_R8() => Emit(OpCodes.Conv_R8);

#endregion

#region *byte

    public S Cpblk() => Emit(OpCodes.Cpblk);

    public S Initblk() => Emit(OpCodes.Initblk);

    public S Localloc() => Emit(OpCodes.Localloc);

#endregion

#region Stack Manip.

    public S Dup() => Emit(OpCodes.Dup);
    public S Pop() => Emit(OpCodes.Pop);

#endregion

#region Create/Init

    public S Initobj(Type type) => Emit(OpCodes.Initobj, type);

    public S Initobj<T>() where T : struct
        => Initobj(typeof(T));

    public S Newobj(ConstructorInfo ctor) => Emit(OpCodes.Newobj, ctor);

#endregion

#region Args/Params

#region Load Argument

    public S Ldarg_0() => Emit(OpCodes.Ldarg_0);

    public S Ldarg_1() => Emit(OpCodes.Ldarg_1);

    public S Ldarg_2() => Emit(OpCodes.Ldarg_2);

    public S Ldarg_3() => Emit(OpCodes.Ldarg_3);

    public S Ldarg_S(byte index)
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

    public S Ldarg(ushort index)
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

    public S Ldarga(ushort index)
    {
        if (index <= byte.MaxValue)
            return Emit(OpCodes.Ldarga_S, (byte)index);
        return Emit(OpCodes.Ldarga, index);
    }

    public S Ldarga_S(byte index) => Emit(OpCodes.Ldarga_S, index);

#endregion

#region Store in Argument

    public S Starg(ushort index)
    {
        if (index <= byte.MaxValue)
            return Emit(OpCodes.Starg_S, (byte)index);
        return Emit(OpCodes.Starg, index);
    }

    public S Starg_S(byte index) => Emit(OpCodes.Starg_S, index);

#endregion

#endregion

#region Load Constant|Value

    public S Ldc_I4_M1() => Emit(OpCodes.Ldc_I4_M1);

    public S Ldc_I4_0() => Emit(OpCodes.Ldc_I4_0);

    public S Ldc_I4_1() => Emit(OpCodes.Ldc_I4_1);

    public S Ldc_I4_2() => Emit(OpCodes.Ldc_I4_2);

    public S Ldc_I4_3() => Emit(OpCodes.Ldc_I4_3);

    public S Ldc_I4_4() => Emit(OpCodes.Ldc_I4_4);

    public S Ldc_I4_5() => Emit(OpCodes.Ldc_I4_5);

    public S Ldc_I4_6() => Emit(OpCodes.Ldc_I4_6);

    public S Ldc_I4_7() => Emit(OpCodes.Ldc_I4_7);

    public S Ldc_I4_8() => Emit(OpCodes.Ldc_I4_8);

    public S Ldc_I4_S(sbyte value)
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

    public S Ldc_I4(int value)
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

    public S Ldc_I8(long value) => Emit(OpCodes.Ldc_I8, value);

    public S Ldc_R4(float value) => Emit(OpCodes.Ldc_R4, value);

    public S Ldc_R8(double value) => Emit(OpCodes.Ldc_R8, value);

    public S Ldnull() => Emit(OpCodes.Ldnull);

    public S Ldstr(string str) => Emit(OpCodes.Ldstr, str);

#endregion

#region Load Token

    public S Ldtoken(Type type) => Emit(OpCodes.Ldtoken, type);

    public S Ldtoken(FieldInfo field) => Emit(OpCodes.Ldtoken, field);

    public S Ldtoken(MethodInfo method) => Emit(OpCodes.Ldtoken, method);

#endregion

#region Arrays

    public S Ldlen() => Emit(OpCodes.Ldlen);

    public S Newarr(Type type) => Emit(OpCodes.Newarr, type);

    public S Newarr<T>()
        => Newarr(typeof(T));

    public S Readonly() => Emit(OpCodes.Readonly);

#region Load array Element

    public S Ldelem_I() => Emit(OpCodes.Ldelem_I);

    public S Ldelem_I1() => Emit(OpCodes.Ldelem_I1);

    public S Ldelem_I2() => Emit(OpCodes.Ldelem_I2);

    public S Ldelem_I4() => Emit(OpCodes.Ldelem_I4);

    public S Ldelem_I8() => Emit(OpCodes.Ldelem_I8);

    public S Ldelem_U1() => Emit(OpCodes.Ldelem_U1);

    public S Ldelem_U2() => Emit(OpCodes.Ldelem_U2);

    public S Ldelem_U4() => Emit(OpCodes.Ldelem_U4);

    public S Ldelem_R4() => Emit(OpCodes.Ldelem_R4);

    public S Ldelem_R8() => Emit(OpCodes.Ldelem_R8);

    public S Ldelem_Ref() => Emit(OpCodes.Ldelem_Ref);

    public S Ldelem(Type type)
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

    public S Ldelem<T>() => Ldelem(typeof(T));

    public S Ldelema(Type type) => Emit(OpCodes.Ldelema, type);

    public S Ldelema<T>() => Ldelema(typeof(T));

#endregion

#region Store in array Element

    public S Stelem_I() => Emit(OpCodes.Stelem_I);

    public S Stelem_I1() => Emit(OpCodes.Stelem_I1);

    public S Stelem_I2() => Emit(OpCodes.Stelem_I2);

    public S Stelem_I4() => Emit(OpCodes.Stelem_I4);

    public S Stelem_I8() => Emit(OpCodes.Stelem_I8);

    public S Stelem_R4() => Emit(OpCodes.Stelem_R4);

    public S Stelem_R8() => Emit(OpCodes.Stelem_R8);

    public S Stelem_Ref() => Emit(OpCodes.Stelem_Ref);

    public S Stelem(Type type)
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

    public S Stelem<T>() => Stelem(typeof(T));

#endregion

#endregion

#region Fields

    public S Ldfld(FieldInfo field)
    {
        Throw.IfNull(field);
        if (field.IsStatic)
            return Emit(OpCodes.Ldsfld, field);
        return Emit(OpCodes.Ldfld, field);
    }

    public S Ldsfld(FieldInfo field) => Ldfld(field);

    public S Ldflda(FieldInfo field)
    {
        Throw.IfNull(field);
        if (field.IsStatic)
            return Emit(OpCodes.Ldsflda, field);
        return Emit(OpCodes.Ldflda, field);
    }
    public S Stfld(FieldInfo field)
    {
        Throw.IfNull(field);
        if (field.IsStatic)
            return Emit(OpCodes.Stsfld, field);
        return Emit(OpCodes.Stfld, field);
    }

    public S Stsfld(FieldInfo field) => Stfld(field);
    public S Ldsflda(FieldInfo field) => Ldflda(field);

#endregion

#region Addressing

    public S Cpobj(Type type) => Emit(OpCodes.Cpobj, type);

    public S Cpobj<T>()
        => Cpobj(typeof(T));

    public S Stobj(Type type) => Emit(OpCodes.Stobj, type);

    public S Stobj<T>() => Stobj(typeof(T));

    public S Unaligned(int alignment)
    {
        if (alignment is not (1 or 2 or 4))
            throw new ArgumentOutOfRangeException(nameof(alignment), alignment, "Alignment must be 1, 2, or 4");
        return Emit(OpCodes.Unaligned, alignment);
    }

    public S Volatile() => Emit(OpCodes.Volatile);

#region Load

    public S Ldind_I() => Emit(OpCodes.Ldind_I);

    public S Ldind_I1() => Emit(OpCodes.Ldind_I1);

    public S Ldind_I2() => Emit(OpCodes.Ldind_I2);

    public S Ldind_I4() => Emit(OpCodes.Ldind_I4);

    public S Ldind_I8() => Emit(OpCodes.Ldind_I8);

    public S Ldind_U1() => Emit(OpCodes.Ldind_U1);

    public S Ldind_U2() => Emit(OpCodes.Ldind_U2);

    public S Ldind_U4() => Emit(OpCodes.Ldind_U4);

    public S Ldind_R4() => Emit(OpCodes.Ldind_R4);

    public S Ldind_R8() => Emit(OpCodes.Ldind_R8);

    public S Ldind_Ref() => Emit(OpCodes.Ldind_Ref);

    public S Ldobj(Type type) => Emit(OpCodes.Ldobj, type);

    public S Ldobj<T>() => Ldobj(typeof(T));

#endregion

#region Store

    public S Stind_I() => Emit(OpCodes.Stind_I);

    public S Stind_I1() => Emit(OpCodes.Stind_I1);

    public S Stind_I2() => Emit(OpCodes.Stind_I2);

    public S Stind_I4() => Emit(OpCodes.Stind_I4);

    public S Stind_I8() => Emit(OpCodes.Stind_I8);

    public S Stind_R4() => Emit(OpCodes.Stind_R4);

    public S Stind_R8() => Emit(OpCodes.Stind_R8);

    public S Stind_Ref() => Emit(OpCodes.Stind_Ref);

#endregion

#endregion

#region Locals

#region Load Local

    public S Ldloc_0() => Emit(OpCodes.Ldloc_0);

    public S Ldloc_1() => Emit(OpCodes.Ldloc_1);

    public S Ldloc_2() => Emit(OpCodes.Ldloc_2);

    public S Ldloc_3() => Emit(OpCodes.Ldloc_3);

    public S Ldloc_S(byte index) => Emit(OpCodes.Ldloc_S, index);

    public S Ldloc(ushort index)
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

    public S Ldloc(CILLocal local)
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

    public S Ldloc_S(CILLocal local) => Ldloc(local);

    public S Ldloca(CILLocal local)
    {
        ValidateLocal(local).ThrowIfError();
        if (local.IsShortForm)
            return Emit(OpCodes.Ldloca_S, local);
        return Emit(OpCodes.Ldloca, local);
    }

    public S Ldloca_S(CILLocal local) => Ldloca(local);

#endregion

#region Store Local

    public S Stloc_0() => Emit(OpCodes.Stloc_0);
    public S Stloc_1() => Emit(OpCodes.Stloc_1);
    public S Stloc_2() => Emit(OpCodes.Stloc_2);
    public S Stloc_3() => Emit(OpCodes.Stloc_3);

    public S Stloc_S(byte index)
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

    public S Stloc(ushort index)
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


    public S Stloc(CILLocal local)
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

    public S Stloc_S(CILLocal local) => Stloc(local);

#endregion

#endregion

#region Type/Ref

    public S Mkrefany(Type type) => Emit(OpCodes.Mkrefany, type);

    public S Mkrefany<T>()
        => Mkrefany(typeof(T));

    public S Refanytype() => Emit(OpCodes.Refanytype);

    public S Refanyval(Type type) => Emit(OpCodes.Refanyval, type);

    public S Refanyval<T>()
        => Refanyval(typeof(T));

    public S Sizeof(Type type) => Emit(OpCodes.Sizeof, type);

    public S Sizeof<T>()
        where T : struct
        => Sizeof(typeof(T));

#endregion

#endregion
}