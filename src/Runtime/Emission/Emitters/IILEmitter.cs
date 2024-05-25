using ScrubJay.Reflection.Runtime.Emission.Instructions;

namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

public interface IILEmitter
{
    /// <summary>
    /// Gets the <see cref="InstructionStream">Stream</see> of <see cref="Instruction">Instructions</see> emitted thus far
    /// </summary>
    InstructionStream Instructions { get; }
}

/// <summary>
/// A fluent emitter of <see cref="Instruction">Instructions</see>
/// </summary>
/// <typeparam name="TEmitter">
/// The <see cref="Type"/> of the emitter instance returned from fluent operations
/// </typeparam>
public interface IILEmitter<TEmitter> : IILEmitter
    where TEmitter : IILEmitter<TEmitter>
{

}


internal static class InstructionLoader
{
    public static void LoadAll<TEmitter>(
        TEmitter emitter,
        InstructionStream instructions)
        where TEmitter : IOpCodeEmitter<TEmitter>, IGeneratorEmitter<TEmitter>
    {
        foreach (var line in instructions)
        {
            var instruction = line.Instruction;
            if (instruction is ILGeneratorInstruction ilGenInstruction)
            {
                switch (ilGenInstruction)
                {
                    case BeginCatchBlockInstruction bcb:
                        emitter.BeginCatchBlock(bcb.ExceptionType);
                        break;
                    case BeginExceptFilterBlockInstruction:
                        emitter.BeginExceptFilterBlock();
                        break;
                    case BeginExceptionBlockInstruction beb:
                    {
                        var incomingLabel = beb.Label;
                        emitter.BeginExceptionBlock(out var label, incomingLabel.Name);
                        if (!incomingLabel.Equals(label))
                            throw new InvalidOperationException();
                        break;
                    }
                    case EndExceptionBlockInstruction:
                        emitter.EndExceptionBlock();
                        break;
                    case BeginFaultBlockInstruction:
                        emitter.BeginFaultBlock();
                        break;
                    case BeginFinallyBlockInstruction:
                        emitter.BeginFinallyBlock();
                        break;
                    case BeginScopeInstruction:
                        emitter.BeginScope();
                        break;
                    case EndScopeInstruction:
                        emitter.EndScope();
                        break;
                    case UsingNamespaceInstruction ns:
                        emitter.UsingNamespace(ns.Namespace);
                        break;
                    case DeclareLocalInstruction dlInstruction:
                    {
                        var incomingLocal = dlInstruction.Local;
                        emitter.DeclareLocal(incomingLocal.LocalType, incomingLocal.IsPinned, out var local, incomingLocal.Name);
                        if (!incomingLocal.Equals(local))
                            throw new InvalidOperationException();
                        break;
                    }
                    case DefineLabelInstruction dlInstruction:
                    {
                        var incomingLabel = dlInstruction.Label;
                        emitter.DefineLabel(out var label, incomingLabel.Name);
                        if (!incomingLabel.Equals(label))
                            throw new InvalidOperationException();
                        break;
                    }
                    case MarkLabelInstruction mlInstruction:
                        emitter.MarkLabel(mlInstruction.Label);
                        break;
                    case CallVarargsInstruction call:
                        emitter.EmitCall(call.Method, call.OptionalParameterTypes);
                        break;
                    case CallManagedInstruction call:
                        emitter.EmitCalli(call.Conventions, call.ReturnType, call.ParameterTypes, call.OptionalParameterTypes);
                        break;
                    case CallUnmanagedInstruction call:
                        emitter.EmitCalli(call.Convention, call.ReturnType, call.ParameterTypes);
                        break;
                    case WriteLineInstruction writeLine:
                        throw new NotImplementedException();
                        break;
                    case ThrowExceptionInstruction throwException:
                        throw new NotImplementedException();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (instruction is OpCodeInstruction opCodeInstruction)
            {
                switch (opCodeInstruction)
                {
                    case OpCodeArgInstruction<byte> instr:
                        emitter.Emit(instr.OpCode, instr.Arg);
                        break;
                    case OpCodeArgInstruction<sbyte> instr:
                        emitter.Emit(instr.OpCode, instr.Arg);
                        break;
                    case OpCodeArgInstruction<short> instr:
                        emitter.Emit(instr.OpCode, instr.Arg);
                        break;
                    case OpCodeArgInstruction<int> instr:
                        emitter.Emit(instr.OpCode, instr.Arg);
                        break;
                    case OpCodeArgInstruction<long> instr:
                        emitter.Emit(instr.OpCode, instr.Arg);
                        break;
                    case OpCodeArgInstruction<float> instr:
                        emitter.Emit(instr.OpCode, instr.Arg);
                        break;
                    case OpCodeArgInstruction<double> instr:
                        emitter.Emit(instr.OpCode, instr.Arg);
                        break;
                    case OpCodeArgInstruction<string> instr:
                        emitter.Emit(instr.OpCode, instr.Arg);
                        break;
                    case OpCodeArgInstruction<EmitterLabel> instr:
                        emitter.Emit(instr.OpCode, instr.Arg);
                        break;
                    case OpCodeArgInstruction<EmitterLabel[]> instr:
                        emitter.Emit(instr.OpCode, instr.Arg);
                        break;
                    case OpCodeArgInstruction<EmitterLocal> instr:
                        emitter.Emit(instr.OpCode, instr.Arg);
                        break;
                    case OpCodeArgInstruction<FieldInfo> instr:
                        emitter.Emit(instr.OpCode, instr.Arg);
                        break;
                    case OpCodeArgInstruction<ConstructorInfo> instr:
                        emitter.Emit(instr.OpCode, instr.Arg);
                        break;
                    case OpCodeArgInstruction<MethodInfo> instr:
                        emitter.Emit(instr.OpCode, instr.Arg);
                        break;
                    case OpCodeArgInstruction<Type> instr:
                        emitter.Emit(instr.OpCode, instr.Arg);
                        break;
                    case OpCodeArgInstruction<SignatureHelper> instr:
                        emitter.Emit(instr.OpCode, instr.Arg);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}