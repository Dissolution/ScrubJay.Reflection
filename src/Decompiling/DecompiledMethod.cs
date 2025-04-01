using ScrubJay.Memory;
using ScrubJay.Reflection.Collections;
using ScrubJay.Reflection.Naming;
using ScrubJay.Reflection.Runtime.Emission;
using ScrubJay.Reflection.Runtime.Emission.Instructions;

namespace ScrubJay.Reflection.Decompiling;

public sealed class DecompiledMethod
{
    private const byte IsTwoByteOpCode = 0xFE;
    private static readonly OpCode[] _oneByteOpCodes = new OpCode[225];
    private static readonly OpCode[] _twoByteOpCodes = new OpCode[31];

    public static IReadOnlyList<OpCode> AllOpCodes { get; }

    static DecompiledMethod()
    {
        var opCodesFields = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static);
        var opCodes = new List<OpCode>();
        foreach (var field in opCodesFields)
        {
            var opCode = field.GetValue(null).ThrowIfNot<OpCode>();
            opCodes.Add(opCode);
            if (opCode.OpCodeType == OpCodeType.Nternal)
                continue;

            if (opCode.Size == 1)
            {
                Debug.Assert(opCode.Value < 225);
                _oneByteOpCodes[opCode.Value] = opCode;
            }
            else
            {
                Debug.Assert(opCode.Size == 2);
                Debug.Assert((opCode.Value & 0xFF) < 31);
                _twoByteOpCodes[opCode.Value & 0xFF] = opCode;
            }
        }
        AllOpCodes = opCodes;
    }

    private static OpCode ReadOpCode(ref SpanReader<byte> reader)
    {
        byte op = reader.TakeValue<byte>();
        if (op != IsTwoByteOpCode)
        {
            return _oneByteOpCodes[op];
        }
        else
        {
            op = reader.TakeValue<byte>();
            return _twoByteOpCodes[op];
        }
    }

    private static Option<object> ReadOperand(ref SpanReader<byte> reader,
        DecompiledMethod decompiledMethod,
        OpCode opCode)
    {
        switch (opCode.OperandType)
        {
            case OperandType.InlineNone:
                return None<object>();
            case OperandType.InlineSwitch:
            {
                int length = reader.TakeValue<int>();
                int baseOffset = reader.Position + (4 * length);
                int[] branches = new int[length];
                for (int i = 0; i < length; i++)
                {
                    branches[i] = reader.TakeValue<int>() + baseOffset;
                }
                return Some<object>(branches);
            }
            case OperandType.ShortInlineBrTarget:
            {
                var operand = reader.TakeValue<sbyte>() + reader.Position;
                return Some<object>(operand);
            }
            case OperandType.InlineBrTarget:
            {
                var operand = reader.TakeValue<int>() + reader.Position;
                return Some<object>(operand);
            }
            case OperandType.ShortInlineI:
            {
                if (opCode == OpCodes.Ldc_I4_S)
                {
                    var operand = reader.TakeValue<sbyte>();
                    return Some<object>(operand);
                }
                else
                {
                    var operand = reader.TakeValue<byte>();
                    return Some<object>(operand);
                }
            }
            case OperandType.InlineI:
            {
                var operand = reader.TakeValue<int>();
                return Some<object>(operand);
            }
            case OperandType.ShortInlineR:
            {
                var operand = reader.TakeValue<float>();
                return Some<object>(operand);
            }
            case OperandType.InlineR:
            {
                var operand = reader.TakeValue<double>();
                return Some<object>(operand);
            }
            case OperandType.InlineI8:
            {
                var operand = reader.TakeValue<long>();
                return Some<object>(operand);
            }
            case OperandType.InlineSig:
            {
                int metadataToken = reader.TakeValue<int>();
                var sig = decompiledMethod.MethodModule.ResolveSignature(metadataToken);
                return Some<object>(sig);
            }
            case OperandType.InlineString:
            {
                int metadataToken = reader.TakeValue<int>();
                var str = decompiledMethod.MethodModule.ResolveString(metadataToken);
                return Some<object>(str);
            }
            case OperandType.InlineTok:
            case OperandType.InlineType:
            case OperandType.InlineMethod:
            case OperandType.InlineField:
            {
                int metadataToken = reader.TakeValue<int>();
                var member = decompiledMethod.MethodModule.ResolveMember(
                    metadataToken: metadataToken,
                    genericTypeArguments: decompiledMethod.OwnerGenericTypes,
                    genericMethodArguments: decompiledMethod.MethodGenericTypes);
                Throw.IfNull(member);
                return Some<object>(member);
            }
            case OperandType.ShortInlineVar:
            {
                var variable = GetVariable(decompiledMethod, opCode, reader.TakeValue<byte>());
                return Some<object>(variable);
            }
            case OperandType.InlineVar:
            {
                var variable = GetVariable(decompiledMethod, opCode, reader.TakeValue<short>());
                return Some<object>(variable);
            }
            //case OperandType.InlinePhi:
            default:
                throw new NotSupportedException();
        }
    }

    private static bool TargetsLocalVariable(OpCode opcode)
    {
        return MemoryExtensions.Contains(opcode.Name.AsSpan(), "loc".AsSpan(), StringComparison.OrdinalIgnoreCase);
    }

    private static object GetVariable(DecompiledMethod decompiledMethod, OpCode opCode, int index)
    {
        if (TargetsLocalVariable(opCode))
        {
            return decompiledMethod.Locals[index];
        }
        else
        {
            return decompiledMethod.Parameters[index];
        }
    }

    public static Result<DecompiledMethod> TryDecompile(MethodBase method)
    {
        Throw.IfNull(method);
        DecompiledMethod decompiledMethod = new()
        {
            MethodModule = method.Module,
            MethodBase = method,
        };

        var methodBody = method.GetMethodBody();
        if (methodBody is null)
        {
            return new ArgumentException($"Method {method.NameOf()} has no method body", nameof(method));
        }
        decompiledMethod.MethodBody = methodBody;

        var locals = methodBody.LocalVariables;
        decompiledMethod.Locals = locals;

        var ilBytes = methodBody.GetILAsByteArray();
        if (ilBytes is null || ilBytes.Length == 0)
        {
            return new ArgumentException($"Method {method.NameOf()}'s body contains no IL bytes", nameof(method));
        }
        decompiledMethod.ILBytes = ilBytes;

        if (method is MethodInfo methodInfo)
        {
            decompiledMethod.ReturnParameter = methodInfo.ReturnParameter;
        }
        else if (method is ConstructorInfo constructorInfo)
        {
            if (constructorInfo.IsStatic)
            {
                decompiledMethod.ReturnParameter = new ReturnParameterInfo(constructorInfo, typeof(void));
            }
            else
            {

                decompiledMethod.ReturnParameter = new ReturnParameterInfo(constructorInfo, constructorInfo.DeclaringType.ThrowIfNull());
            }
        }

        if (!method.IsStatic)
        {
            // Method has `this` as its first parameter
            var methodParams = method.GetParameters();
            decompiledMethod.Parameters = [new ThisParameterInfo(method), ..methodParams];
        }
        else
        {
            decompiledMethod.Parameters = method.GetParameters();
        }

        var ilStream = new InstructionStream<OpCodeInstruction>();
        var reader = new SpanReader<byte>(ilBytes);
        while (reader.RemainingCount > 0)
        {
            int pos = reader.Position;
            OpCode opCode = ReadOpCode(ref reader);
            Option<object> operand = ReadOperand(ref reader, decompiledMethod, opCode);
            var instr = new OpCodeInstruction(opCode)
            {
                Offset = pos,
                Operand = operand,
            };
            ilStream.Add(instr);
        }

        foreach (var instruction in ilStream)
        {
            switch (instruction.OpCode.OperandType)
            {
                case OperandType.ShortInlineBrTarget:
                case OperandType.InlineBrTarget:
                {
                    int offset = instruction.Operand.SomeOrThrow().ThrowIfNot<int>();
                    var operand = ilStream.TryFindByOffset(offset).SomeOrThrow();
                    instruction.Operand = Some<object>(operand);
                    break;
                }
                case OperandType.InlineSwitch:
                {
                    int[] offsets = instruction.Operand.SomeOrThrow().ThrowIfNot<int[]>();
                    var branches = new Instruction[offsets.Length];
                    for (int j = 0; j < offsets.Length; j++)
                    {
                        branches[j] = ilStream.TryFindByOffset(offsets[j]).SomeOrThrow();
                    }
                    instruction.Operand = Some<object>(branches);
                    break;
                }
            }
        }

        decompiledMethod.OpCodeInstructions = ilStream;
        return Ok(decompiledMethod);
    }



    private sealed class ThisParameterInfo : ParameterInfo
    {
        public ThisParameterInfo(MethodBase method)
        {
            Debug.Assert(!method.IsStatic);
            this.MemberImpl = method;
            this.ClassImpl = method.DeclaringType.ThrowIfNull();
            this.NameImpl = "this";
            this.PositionImpl = 0;
        }
    }

    private sealed class ReturnParameterInfo : ParameterInfo
    {
        public override ParameterAttributes Attributes { get; } = ParameterAttributes.Retval;

        public ReturnParameterInfo(MethodBase method, Type returnType)
        {
            this.MemberImpl = method;
            this.ClassImpl = returnType;
            this.NameImpl = "return";
            this.PositionImpl = -1;
        }
    }

    public Module MethodModule { get; private set; }
    public Type OwnerType => MethodBase.OwnerType();
    public Type[]? OwnerGenericTypes => OwnerType.GetGenericArguments().NullIfNone();
    public MethodBase MethodBase { get; private set; }
    public Type[]? MethodGenericTypes => MethodBase.GetGenericArguments().NullIfNone();
    public MethodBody MethodBody { get; private set; }
    public byte[] ILBytes { get; private set; }

    public ParameterInfo? ReturnParameter { get; private set; }
    public ParameterInfo[] Parameters { get; private set; }

    public IList<LocalVariableInfo> Locals { get; private set; }

    public InstructionStream<OpCodeInstruction> OpCodeInstructions { get; private set; }

    private DecompiledMethod()
    {

    }

    public override string ToString() => TextBuilder.New
        .If(Validate.IsNotEmpty(Locals),
            static (tb, locals) => tb.AppendLine(".locals")
                .Enumerate(locals, static (t, l) => t.AppendLine(new EmitterLocal(l))))
        .Delimit(static tb => tb.NewLine(), OpCodeInstructions, static (tb, instr) => tb.Append(instr))
        .ToStringAndDispose();

}
