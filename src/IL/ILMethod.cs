using ScrubJay.Reflection.IL.Instructions;

namespace ScrubJay.Reflection.IL;

public abstract class ILMethod
{
    private Type[]? _parameterTypes;
    private readonly InstructionStream _instructionStream = [];

    protected ILLocal? LocalOrNull(int index)
    {
        if ((uint)index < (uint)Locals.Count)
            return Locals[index];
        return null;
    }

    protected ILLocal LocalOrThrow(int index)
    {
        if ((uint)index < (uint)Locals.Count)
            return Locals[index];
        throw new ArgumentOutOfRangeException(nameof(index), index, $"Local at index [{index}] does not exist");
    }

    protected ParameterInfo? ParameterOrNull(int index)
    {
        if ((uint)index < (uint)Parameters.Length)
            return Parameters[index];
        return null;
    }

    protected ParameterInfo ParameterOrThrow(int index)
    {
        if ((uint)index < (uint)Parameters.Length)
            return Parameters[index];
        throw new ArgumentOutOfRangeException(nameof(index), index, $"Parameter at index [{index}] does not exist");
    }

    protected virtual OpCodeInstruction Inflate(OpCodeInstruction instruction)
    {
        var opCode = instruction.OpCode;
        if (opCode.OperandType == OperandType.InlineNone)
        {
            if (opCode.TargetsLocal().Flatten().IsSome(out int index))
            {
                return new OpCodeLocalInstruction(opCode, index)
                {
                    Local = LocalOrThrow(index),
                };
            }
            
            if (opCode.TargetsArgument().Flatten().IsSome(out index))
            {
                return new OpCodeParameterInstruction(opCode, index)
                {
                    Parameter = ParameterOrThrow(index),
                };
            }
            
            if (opCode.TargetsI32Const().IsSome(out index))
            {
                return new OpCodeValueInstruction<int>(opCode, index);
            }

            return instruction;
        }
        
        if (instruction is OpCodeLocalInstruction localInstr)
        {
            localInstr.Local ??= LocalOrThrow(localInstr.Index);
            return localInstr;
        }
        
        if (instruction is OpCodeParameterInstruction paramInstr)
        {
            paramInstr.Parameter ??= ParameterOrThrow(paramInstr.Index);
            return paramInstr;
        }

        if (instruction is OpCodeBranchInstruction branchInstr)
        {
            var foundLabel = Labels.TryGetOne(lbl => lbl.Offset == branchInstr.TargetOffset);
            if (!foundLabel.IsOk(out var label))
            {
                label = new ILLabel(Labels.Count, branchInstr.TargetOffset);
            }

            branchInstr.Label = label;
        }
        
        return instruction;
    }

    protected virtual void AddInstruction(Instruction instruction)
    {
        Instruction toAdd;
        if (instruction is OpCodeInstruction opCodeInstruction)
        {
            toAdd = Inflate(opCodeInstruction);
        }
        else if (instruction is ILGeneratorInstruction ilGenInstruction)
        {
            Debugger.Break();
            toAdd = ilGenInstruction;
        }
        else
        {
            Debugger.Break();
            toAdd = instruction;
        }
        
        _instructionStream.Add(toAdd);
    }

    public required Type OwnerType { get; init; }
    public Type[] OwnerGenericTypes => OwnerType.GenericTypes() ?? [];

    
    public required MethodAttributes MethodAttributes { get; init; }

    public bool IsStatic => MethodAttributes.HasFlags(MethodAttributes.Static);
    
    public string? Name { get; init; } = null;
    
    public Type[] GenericTypes { get; init; } = [];
    public required ParameterInfo ReturnParameter { get; init; }
    public required ParameterInfo[] Parameters { get; init; }
    public Type[] ParameterTypes => _parameterTypes ??= Parameters.ConvertAll(static p => p.ParameterType);

    public abstract IReadOnlyList<ILLocal> Locals { get; init;}
    public abstract IReadOnlyList<ILLabel> Labels { get; init;}

    public IInstructions Instructions => _instructionStream;

}