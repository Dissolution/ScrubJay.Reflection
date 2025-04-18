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

public sealed class Emitter : EmitterBase<Emitter>
{
    public Emitter(ILGenerator ilGenerator) : base(ilGenerator)
    {
    }
}

public abstract class EmitterBase<E> : FluentBuilder<E>,
    IEmitter<E>,
    IOpCodeEmitter<E>,
    IGenEmitter<E>,
    IOperationEmitter<E>
    where E : EmitterBase<E>
{
    private readonly ILGenerator? _ilGenerator;

    private readonly Dictionary<ILLabel, Label> _labels = [];
    private readonly Dictionary<ILLocal, LocalBuilder> _locals = [];

    public InstructionStream Instructions { get; } = [];

    public IReadOnlyCollection<ILLabel> Labels => _labels.Keys;

    public IReadOnlyCollection<ILLocal> Locals => _locals.Keys;

    protected EmitterBase(ILGenerator? ilGenerator)
    {
        _ilGenerator = ilGenerator;
    }

    protected Result<Label> ValidateLabel(
        ILLabel ilLabel,
        [CallerArgumentExpression(nameof(ilLabel))]
        string? CILLabelName = null)
    {
        if (!_labels.TryGetValue(ilLabel, out var label))
            return new ArgumentException($"CILLabel '{ilLabel}' does not belong to this Emitter", CILLabelName);
        return label;
    }

    protected Result<LocalBuilder> ValidateLocal(
        ILLocal ilLocal,
        [CallerArgumentExpression(nameof(ilLocal))]
        string? CILLocalName = null)
    {
        if (!_locals.TryGetValue(ilLocal, out var local))
            return new ArgumentException($"CILLocal '{ilLocal}' does not belong to this Emitter", CILLocalName);
        return local;
    }

    protected Result<ILLocal> ValidateLocal(int index)
    {
        return _locals.Keys.Where(l => l.Index == index).TryGetOne();
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

    public E Emit(OpCode opCode)
    {
        _ilGenerator?.Emit(opCode);

        if (opCode.TargetsLocal().Flatten().IsSome(out var index))
        {
            var instruction = new OpCodeLocalInstruction(opCode, index);
            if (ValidateLocal(index).IsOk(out var local))
            {
                instruction.Local = local;
            }
            Instructions.Add(instruction);
        }
        else if (opCode.TargetsArgument().Flatten().IsSome(out index))
        {
            var instruction = new OpCodeParameterInstruction(opCode, index);
            Instructions.Add(instruction);
        }
        else
        {
            Instructions.Add(new OpCodeInstruction(opCode));
        }
       
        return _builder;
    }

    public E Emit(OpCode opCode, byte u8)
    {
        _ilGenerator?.Emit(opCode, u8);

        Debugger.Break();
        return _builder;
    }

    public E Emit(OpCode opCode, sbyte i8)
    {
        _ilGenerator?.Emit(opCode, i8);
        Debugger.Break();
        return _builder;
    }

    public E Emit(OpCode opCode, short i16)
    {
        _ilGenerator?.Emit(opCode, i16);
        Debugger.Break();
        return _builder;
    }

    public E Emit(OpCode opCode, int i32)
    {
        _ilGenerator?.Emit(opCode, i32);
        Debugger.Break();
        return _builder;
    }

    public E Emit(OpCode opCode, long i64)
    {
        _ilGenerator?.Emit(opCode, i64);
        Debugger.Break();
        return _builder;
    }

    public E Emit(OpCode opCode, float f32)
    {
        _ilGenerator?.Emit(opCode, f32);
        Debugger.Break();
        return _builder;
    }

    public E Emit(OpCode opCode, double i8)
    {
        _ilGenerator?.Emit(opCode, i8);
        Debugger.Break();
        return _builder;
    }

    public E Emit(OpCode opCode, string str)
    {
        Throw.IfNull(str);
        _ilGenerator?.Emit(opCode, str);
        Debugger.Break();
        return _builder;
    }

    public E Emit(OpCode opCode, ILLabel label)
    {
        var lbl = ValidateLabel(label).OkOrThrow();
        _ilGenerator?.Emit(opCode, lbl);
        Instructions.Add(new OpCodeBranchInstruction(opCode, label));
        return _builder;
    }

    public E Emit(OpCode opCode, params ILLabel[] cilLabels)
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

    public E Emit(OpCode opCode, ILLocal local)
    {
        var lcl = ValidateLocal(local).OkOrThrow();
        _ilGenerator?.Emit(opCode, lcl);
        Instructions.Add(new OpCodeLocalInstruction(opCode, local.Index)
        {
            Local = local,
        });
        return _builder;
    }

    public E Emit(OpCode opCode, FieldInfo field)
    {
        Throw.IfNull(field);
        _ilGenerator?.Emit(opCode, field);
        Instructions.Add(new OpCodeFieldInstruction(opCode, field.MetadataToken)
        {
            Field = field,
        });
        return _builder;
    }

    public E Emit(OpCode opCode, ConstructorInfo ctor)
    {
        Throw.IfNull(ctor);
        _ilGenerator?.Emit(opCode, ctor);
        Debugger.Break();
        return _builder;
    }

    public E Emit(OpCode opCode, MethodInfo method)
    {
        Throw.IfNull(method);
        _ilGenerator?.Emit(opCode, method);
        Instructions.Add(new OpCodeMethodInstruction(opCode, method.MetadataToken)
        {
            Method = method,
        });
        return _builder;
    }

    public E Emit(OpCode opCode, Type type)
    {
        Throw.IfNull(type);
        _ilGenerator?.Emit(opCode, type);
        Instructions.Add(new OpCodeTypeInstruction(opCode, type.MetadataToken)
        {
            Type = type,
        });
        return _builder;
    }

    public E Emit(OpCode opCode, SignatureHelper signature)
    {
        Throw.IfNull(signature);
        _ilGenerator?.Emit(opCode, signature);
        Debugger.Break();
        return _builder;
    }

#endregion

#region IGenEmitter

    public E BeginExceptionBlock(out ILLabel label,
        [CallerArgumentExpression(nameof(label))]
        string? labelName = null)
    {
        Label lbl;
        if (_ilGenerator is null)
        {
            lbl = ILLabel.CreateLabel(_labels.Count);
        }
        else
        {
            lbl = _ilGenerator.BeginExceptionBlock();
        }
        label = new ILLabel(lbl, GetVariableName(labelName));
        _labels.Add(label, lbl);
        Instructions.Add(new ILGeneratorBeginExceptionBlockInstruction(label));
        return _builder;
    }

    public E BeginCatchBlock(Type exceptionType)
    {
        Throw.IfNull(exceptionType);
        if (!exceptionType.Implements<Exception>())
            throw new ArgumentException($"Exception Type '{exceptionType.NameOf()}' is not an Exception", nameof(exceptionType));
        _ilGenerator?.BeginCatchBlock(exceptionType);
        Instructions.Add(new ILGeneratorBeginCatchBlockInstruction(exceptionType));
        return _builder;
    }

    public E BeginCatchBlock<TException>()
        where TException : Exception
        => BeginCatchBlock(typeof(TException));

    public E BeginFinallyBlock()
    {
        _ilGenerator?.BeginFinallyBlock();
        Instructions.Add(new ILGeneratorInstruction(ILGeneratorMethod.BeginFinallyBlock));
        return _builder;
    }

    public E BeginExceptFilterBlock()
    {
        _ilGenerator?.BeginExceptFilterBlock();
        Instructions.Add(new ILGeneratorInstruction(ILGeneratorMethod.BeginExceptFilterBlock));
        return _builder;
    }

    public E BeginFaultBlock()
    {
        _ilGenerator?.BeginFaultBlock();
        Instructions.Add(new ILGeneratorInstruction(ILGeneratorMethod.BeginFaultBlock));
        return _builder;
    }

    public E EndExceptionBlock()
    {
        _ilGenerator?.EndExceptionBlock();
        Instructions.Add(new ILGeneratorInstruction(ILGeneratorMethod.EndExceptionBlock));
        return _builder;
    }

    public E BeginScope()
    {
        _ilGenerator?.BeginScope();
        Instructions.Add(new ILGeneratorInstruction(ILGeneratorMethod.BeginScope));
        return _builder;
    }

    public E EndScope()
    {
        _ilGenerator?.EndScope();
        Instructions.Add(new ILGeneratorInstruction(ILGeneratorMethod.EndScope));
        return _builder;
    }

    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/namespaces#143-namespace-declarations"/>
    public E UsingNamespace(string @namespace)
    {
        Throw.IfEmpty(@namespace);
        if (!CodeHelper.IsValidNamespace(@namespace))
            throw new ArgumentException($"Namespace '{@namespace}' is not valid", nameof(@namespace));
        _ilGenerator?.UsingNamespace(@namespace);
        Instructions.Add(new ILGeneratorUsingNamespaceInstruction(@namespace));
        return _builder;
    }

    public E DeclareLocal(Type localType, out ILLocal local,
        [CallerArgumentExpression(nameof(local))]
        string? localName = null)
    {
        var localBuilder = _ilGenerator!.DeclareLocal(localType);
        local = new ILLocal(localBuilder, GetVariableName(localName));
        _locals.Add(local, localBuilder);
        Instructions.Add(new ILGeneratorDeclareLocalInstruction(local));
        return _builder;
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
        local = new ILLocal(localBuilder, GetVariableName(localName));
        _locals.Add(local, localBuilder);
        Instructions.Add(new ILGeneratorDeclareLocalInstruction(local));
        return _builder;
    }

    public E DeclareLocal<T>(bool pinned, out ILLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null)
        => DeclareLocal(typeof(T), pinned, out local, localName);

    public E DefineLabel(out ILLabel label,
        [CallerArgumentExpression(nameof(label))]
        string? labelName = null)
    {
        Label lbl;
        if (_ilGenerator is null)
        {
            lbl = ILLabel.CreateLabel(_labels.Count);
        }
        else
        {
            lbl = _ilGenerator.DefineLabel();
        }

        label = new ILLabel(lbl, GetVariableName(labelName));
        _labels.Add(label, lbl);
        Instructions.Add(new ILGeneratorDefineLabelInstruction(label));
        return _builder;
    }

    public E MarkLabel(ILLabel label)
    {
        var lbl = ValidateLabel(label).OkOrThrow();
        _ilGenerator?.MarkLabel(lbl);
        Instructions.Add(new ILGeneratorMarkLabelInstruction(label));
        return _builder;
    }

    public E EmitCall(MethodInfo methodInfo, Type[]? optionalParameterTypes = null)
    {
        Throw.IfNull(methodInfo);
        var callOpCode = methodInfo.GetCallOpCode();

        _ilGenerator?.EmitCall(callOpCode, methodInfo, optionalParameterTypes);
        Instructions.Add(new ILGeneratorCallVarargsInstruction(callOpCode, methodInfo, optionalParameterTypes));
        return _builder;
    }

    public E EmitCalli(
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
    public E EmitCalli(CallingConvention unmanagedCallConv, Type? returnType, Type[]? parameterTypes)
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

    public E Beq(ILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Beq_S, label);
        return Emit(OpCodes.Beq, label);
    }

    public E Beq_S(ILLabel label) => Beq(label);

    public E Bge(ILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Bge_S, label);
        return Emit(OpCodes.Bge, label);
    }

    public E Bge_S(ILLabel label) => Bge(label);

    public E Bge_Un(ILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Bge_Un_S, label);
        return Emit(OpCodes.Bge_Un, label);
    }

    public E Bge_Un_S(ILLabel label) => Bge_Un(label);

    public E Bgt(ILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Bgt_S, label);
        return Emit(OpCodes.Bgt, label);
    }

    public E Bgt_S(ILLabel label) => Bgt(label);

    public E Bgt_Un(ILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Bgt_Un_S, label);
        return Emit(OpCodes.Bgt_Un, label);
    }

    public E Bgt_Un_S(ILLabel label) => Bgt_Un(label);

    public E Ble(ILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Ble_S, label);
        return Emit(OpCodes.Ble, label);
    }

    public E Ble_S(ILLabel label) => Ble(label);

    public E Ble_Un(ILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Ble_Un_S, label);
        return Emit(OpCodes.Ble_Un, label);
    }

    public E Ble_Un_S(ILLabel label) => Ble_Un(label);

    public E Blt(ILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Blt_S, label);
        return Emit(OpCodes.Blt, label);
    }

    public E Blt_S(ILLabel label) => Blt(label);

    public E Blt_Un(ILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Blt_Un_S, label);
        return Emit(OpCodes.Blt_Un, label);
    }

    public E Blt_Un_S(ILLabel label) => Blt_Un(label);

    public E Bne_Un(ILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Bne_Un_S, label);
        return Emit(OpCodes.Bne_Un, label);
    }

    public E Bne_Un_S(ILLabel label) => Bne_Un(label);

    public E Brfalse(ILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Brfalse_S, label);
        return Emit(OpCodes.Brfalse, label);
    }

    public E Brfalse_S(ILLabel label) => Brfalse(label);

    public E Brtrue(ILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Brtrue_S, label);
        return Emit(OpCodes.Brtrue, label);
    }

    public E Brtrue_S(ILLabel label) => Brtrue(label);

#endregion

#region Unconditional

    public E Br(ILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Br_S, label);
        return Emit(OpCodes.Br, label);
    }

    public E Br_S(ILLabel label) => Br(label);

    public E Leave(ILLabel label)
    {
        ValidateLabel(label).ThrowIfError();
        if (label.IsShortForm)
            return Emit(OpCodes.Leave_S, label);
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

    public E Ldarga(ushort index)
    {
        if (index <= byte.MaxValue)
            return Emit(OpCodes.Ldarga_S, (byte)index);
        return Emit(OpCodes.Ldarga, index);
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

    public E Stelem<T>() => Stelem(typeof(T));

#endregion

#endregion

#region Fields

    public E Ldfld(FieldInfo field)
    {
        Throw.IfNull(field);
        if (field.IsStatic)
            return Emit(OpCodes.Ldsfld, field);
        return Emit(OpCodes.Ldfld, field);
    }

    public E Ldsfld(FieldInfo field) => Ldfld(field);

    public E Ldflda(FieldInfo field)
    {
        Throw.IfNull(field);
        if (field.IsStatic)
            return Emit(OpCodes.Ldsflda, field);
        return Emit(OpCodes.Ldflda, field);
    }
    public E Stfld(FieldInfo field)
    {
        Throw.IfNull(field);
        if (field.IsStatic)
            return Emit(OpCodes.Stsfld, field);
        return Emit(OpCodes.Stfld, field);
    }

    public E Stsfld(FieldInfo field) => Stfld(field);
    public E Ldsflda(FieldInfo field) => Ldflda(field);

#endregion

#region Addressing

    public E Cpobj(Type type) => Emit(OpCodes.Cpobj, type);

    public E Cpobj<T>()
        => Cpobj(typeof(T));

    public E Stobj(Type type) => Emit(OpCodes.Stobj, type);

    public E Stobj<T>() => Stobj(typeof(T));

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

    public E Ldloc_S(ILLocal local) => Ldloc(local);

    public E Ldloca(ILLocal local)
    {
        ValidateLocal(local).ThrowIfError();
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

    public E LoadConst<T>(T? value)
    {
        if (value is null)
            return Ldnull();
        if (value is string str)
            return Ldstr(str);
        if (value is int i32)
            return Ldc_I4(i32);
        if (value is sbyte i8)
            return Ldc_I4_S(i8);
        throw new NotImplementedException();
    }

    public E MarkLabel(out ILLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null)
    {
        return DefineLabel(out label, labelName)
            .MarkLabel(label);
    }

    [Flags]
    public enum Comparison
    {
        NotEqual = 0,

        Equal = 1 << 0,

        LessThan = 1 << 1,

        GreaterThan = 1 << 2,

        LessThanOrEqual = LessThan | Equal,

        GreaterThanOrEqual = GreaterThan | Equal,

        Unconditional = Equal | LessThan | GreaterThan,
    }

    public E Branch(Comparison comparison, ILLabel label)
    {
        if (comparison is Comparison.NotEqual or (Comparison.LessThan | Comparison.GreaterThan))
            return Bne_Un(label);
        if (comparison == Comparison.Equal)
            return Beq(label);
        if (comparison == Comparison.LessThan)
            return Blt(label);
        if (comparison == Comparison.LessThanOrEqual)
            return Ble(label);
        if (comparison == Comparison.GreaterThan)
            return Bgt(label);
        if (comparison == Comparison.GreaterThanOrEqual)
            return Bge(label);
        if (comparison == Comparison.Unconditional)
            return Br(label);
        throw InvalidEnumException.Create(comparison);
    }

    public TryCatchFinally<E> Try(Action<E, ILLabel> tryBlock)
    {
        return new TryCatchFinally<E>(_builder)
            .Try(tryBlock);
    }

    public E Enumerate<T>(T[]? array, Action<E, T> emitItem)
    {
        var emitter = _builder;
        if (array is not null)
        {
           
            int len = array.Length;
            for (var i = 0; i < len; i++)
            {
                emitItem(emitter, array[i]);
            }
        }
        return emitter;
    }
    
    
#endregion
}