using ScrubJay.Reflection.IL.Emission;

namespace ScrubJay.Reflection.IL.Instructions;

public static class InstructionLoader
{
    private static void LoadGenInstructionTo<E>(ILGeneratorInstruction genInstr, E emitter)
        where E : IOpCodeEmitter<E>, IGenEmitter<E>
    {
        switch (genInstr)
        {
            case ILGeneratorBeginCatchBlockInstruction bcb:
                emitter.BeginCatchBlock(bcb.ExceptionType!);
                break;
            case ILGeneratorBeginExceptionBlockInstruction beb:
            {
                var incomingLabel = beb.Label;
                emitter.BeginExceptionBlock(out var label, incomingLabel.Name);
                if (!incomingLabel.Equals(label))
                    throw new InvalidOperationException();
                break;
            }
            case ILGeneratorUsingNamespaceInstruction ns:
                emitter.UsingNamespace(ns.Namespace);
                break;
            case ILGeneratorDeclareLocalInstruction dlInstruction:
            {
                var incomingLocal = dlInstruction.Local;
                emitter.DeclareLocal(incomingLocal.Type, incomingLocal.IsPinned, out var local, incomingLocal.Name);
                if (!incomingLocal.Equals(local))
                    throw new InvalidOperationException();
                break;
            }
            case ILGeneratorDefineLabelInstruction dlInstruction:
            {
                var incomingLabel = dlInstruction.Label;
                emitter.DefineLabel(out var label, incomingLabel.Name);
                if (!incomingLabel.Equals(label))
                    throw new InvalidOperationException();
                break;
            }
            case ILGeneratorMarkLabelInstruction mlInstruction:
                emitter.MarkLabel(mlInstruction.Label);
                break;
            case ILGeneratorCallVarargsInstruction call:
                emitter.EmitCall(call.Method, call.OptionalParameterTypes);
                break;
            case ILGeneratorCallManagedInstruction call:
                emitter.EmitCalli(call.CallingConventions, call.ReturnType, call.ParameterTypes, call.OptionalParameterTypes);
                break;
#if !NETSTANDARD2_0
            case ILGeneratorCallUnmanagedInstruction call:
                emitter.EmitCalli(call.CallingConvention, call.ReturnType, call.ParameterTypes);
                break;
#endif
            case ILGeneratorWriteLineInstruction writeLine:
                throw new NotImplementedException();
                break;
            case ILGeneratorThrowExceptionInstruction throwException:
                throw new NotImplementedException();
                break;
            default:
            {
                switch (genInstr.ILGenMethod)
                {
                    case ILGeneratorMethod.BeginExceptFilterBlock:
                        emitter.BeginExceptFilterBlock();
                        break;
                    case ILGeneratorMethod.EndExceptionBlock:
                        emitter.EndExceptionBlock();
                        break;
                    case ILGeneratorMethod.BeginFaultBlock:
                        emitter.BeginFaultBlock();
                        break;
                    case ILGeneratorMethod.BeginFinallyBlock:
                        emitter.BeginFinallyBlock();
                        break;
                    case ILGeneratorMethod.BeginScope:
                        emitter.BeginScope();
                        break;
                    case ILGeneratorMethod.EndScope:
                        emitter.EndScope();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(genInstr));
                }
                break;
            }
        }
    }

    private static void LoadOpCodeInstructionTo<E>(OpCodeInstruction opCodeInstr, E emitter)
        where E : IOpCodeEmitter<E>, IGenEmitter<E>
    {
        switch (opCodeInstr)
        {
            case OpCodeValueInstruction<byte> instr:
                emitter.Emit(instr.OpCode, instr.Value);
                break;
            case OpCodeValueInstruction<sbyte> instr:
                emitter.Emit(instr.OpCode, instr.Value);
                break;
            case OpCodeValueInstruction<short> instr:
                emitter.Emit(instr.OpCode, instr.Value);
                break;
            case OpCodeValueInstruction<int> instr:
                emitter.Emit(instr.OpCode, instr.Value);
                break;
            case OpCodeValueInstruction<long> instr:
                emitter.Emit(instr.OpCode, instr.Value);
                break;
            case OpCodeValueInstruction<float> instr:
                emitter.Emit(instr.OpCode, instr.Value);
                break;
            case OpCodeValueInstruction<double> instr:
                emitter.Emit(instr.OpCode, instr.Value);
                break;
            case OpCodeStringInstruction instr:
                emitter.Emit(instr.OpCode, instr.String!);
                break;
            case OpCodeLabelInstruction instr:
                emitter.Emit(instr.OpCode, instr.Label);
                break;
            case OpCodeSwitchInstruction instr:
                //emitter.Emit(instr.OpCode, instr.Deltas);
                throw new NotImplementedException();
                break;
            case OpCodeLocalInstruction instr:
                emitter.Emit(instr.OpCode, instr.Local!.Value);
                break;
            case OpCodeFieldInstruction instr:
                emitter.Emit(instr.OpCode, instr.Field!);
                break;
            case OpCodeMethodInstruction instr:
                if (instr.Method is ConstructorInfo ctor)
                    emitter.Emit(instr.OpCode, ctor);
                else if (instr.Method is MethodInfo meth)
                    emitter.Emit(instr.OpCode, meth);
                break;
            case OpCodeTypeInstruction instr:
                emitter.Emit(instr.OpCode, instr.Type!);
                break;
            case OpCodeSignatureInstruction instr:
                //emitter.Emit(instr.OpCode, instr.Signature!);
                throw new NotImplementedException();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void LoadInstructionsTo<E>(IInstructions instructions, E emitter)
        where E : IOpCodeEmitter<E>, IGenEmitter<E>
    {
        foreach (var instruction in instructions)
        {
            if (instruction is ILGeneratorInstruction ilGenInstruction)
            {
                LoadGenInstructionTo(ilGenInstruction, emitter);
            }
            else if (instruction is OpCodeInstruction opCodeInstruction)
            {
                LoadOpCodeInstructionTo(opCodeInstruction, emitter);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}