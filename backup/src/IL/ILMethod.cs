using ScrubJay.Reflection.IL.Instructions;

namespace ScrubJay.Reflection.IL;

[PublicAPI]
public abstract class ILMethod
{
    private Type[]? _parameterTypes;
    private readonly InstructionStream _instructionStream = [];
    internal protected readonly List<ILLocal> _locals = [];

    public Type? OwnerType { get; init; } = null;
    public Type[] OwnerGenericTypes => OwnerType?.GenericTypes() ?? [];

    public required MethodAttributes MethodAttributes { get; init; }

    public bool IsStatic => MethodAttributes.HasFlags(MethodAttributes.Static);

    public string? Name { get; init; } = null;

    public Type[] GenericTypes { get; init; } = [];
    
    public required ParameterInfo ReturnParameter { get; init; }
    public Type ReturnType => ReturnParameter.ParameterType;
    
    public required ParameterInfo[] Parameters { get; init; }
    public Type[] ParameterTypes => _parameterTypes ??= Parameters.ConvertAll(static p => p.ParameterType);
    public int ParameterCount => Parameters.Length;
    
    public IReadOnlyList<ILLocal> Locals => _locals;
    
    public IInstructions Instructions => _instructionStream;

    protected ILMethod()
    {
        
    }
    
    protected ILLocal? LocalOrNull(int index)
    {
        if ((uint)index < (uint)_locals.Count)
            return _locals[index];
        return null;
    }

    protected ILLocal LocalOrThrow(int index)
    {
        if ((uint)index < (uint)_locals.Count)
            return _locals[index];
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

    protected virtual OpCodeInstruction Inflate(OpCodeInstruction opCodeInstr)
    {
        var opCode = opCodeInstr.OpCode;

        if (opCode.OperandType == OperandType.InlineNone)
        {
            // ldloc.* stloc.*
            if (opCode.TargetsLocal().Flatten().IsSome(out int index))
            {
                return new LocalInstruction(opCode, Locals[index])
                {
                    Offset = opCodeInstr.Offset,
                };
            }

            // ldarg* starg*
            if (opCode.TargetsArgument().Flatten().IsSome(out index))
            {
                return new ParameterInstruction(opCode, Parameters[index])
                {
                    Offset = opCodeInstr.Offset,
                };
            }

            // ldc.i4.*
            if (opCode.TargetsI32Const().IsSome(out var i32))
            {
                return new ValueInstruction<int>(opCode, i32)
                {
                    Offset = opCodeInstr.Offset,
                };
            }

            return new NoneInstruction(opCode)
            {
                Offset = opCodeInstr.Offset,
            };
        }

        return opCodeInstr;
    }
    
    internal void AddInstruction(Instruction instruction)
    {
        // We can inflate certain instructions to contain additional information
        if (instruction is OpCodeInstruction opCodeInstr)
        {
            instruction = Inflate(opCodeInstr);
        }
        _instructionStream.Add(instruction);
    }
    
    public override string ToString()
    {
        return TextBuilder.New
            .IfAppend(MethodAttributes.HasFlags(MethodAttributes.Static), "static ")
            .Render(OwnerType)
            .Append('.')
            .NameGenericsParameters(Name, GenericTypes, Parameters)
            .Append(" => ")
            .Render(ReturnParameter)
            .NewLine()
            .AppendLine("-- Locals")
            .Enumerate(Locals, static (tb, local) => tb.Append($"{local.Index}: {local:@}").NewLine())
            .AppendLine("-- CIL")
            .EnumerateAndLineDelimit(Instructions, static (tb, instr) => instr.RenderTo(tb))
            .ToStringAndDispose();
    }
}