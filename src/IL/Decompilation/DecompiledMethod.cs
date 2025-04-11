using ScrubJay.Reflection.IL.Instructions;
using InvalidOperationException = System.InvalidOperationException;

namespace ScrubJay.Reflection.IL.Decompilation;

public sealed class DecompiledMethod
{
    public static DecompiledMethod Decompile(MethodBase method)
    {
        Throw.IfNull(method);

        return new(method);
    }

    private readonly MethodBase _method;
    private readonly ITokenResolver _tokenResolver;


    private Option<Type[]?> _ownerGenericTypes = None();
    private Option<Type[]?> _methodGenericTypes = None();

    public Type[]? OwnerGenericTypes
    {
        get
        {
            if (!_ownerGenericTypes.IsSome(out var ogts))
            {
                ogts = _method.OwnerType().GetGenericArguments().NullIfNone();
                _ownerGenericTypes = Some(ogts);
            }
            return ogts;
        }
    }

    public Type[]? MethodGenericTypes
    {
        get
        {
            if (!_methodGenericTypes.IsSome(out var mgts))
            {
                mgts = _method.GetGenericArguments().NullIfNone();
                _methodGenericTypes = Some(mgts);
            }
            return mgts;
        }
    }

    public ParameterInfo? ReturnParameter { get; }

    public ParameterInfo[] Parameters { get; }

    public IList<LocalVariableInfo> Locals { get; }


    private DecompiledMethod(MethodBase method)
    {
        _method = method;
        _tokenResolver = ReflectionExtensions.GetTokenResolver(method);

        if (method is MethodInfo methodInfo)
        {
            ReturnParameter = methodInfo.ReturnParameter;
        }
        else if (method is ConstructorInfo constructorInfo)
        {
            if (constructorInfo.IsStatic)
            {
                ReturnParameter = new ReturnParameterInfo(constructorInfo, typeof(void));
            }
            else
            {
                ReturnParameter = new ReturnParameterInfo(constructorInfo, constructorInfo.DeclaringType.ThrowIfNull());
            }
        }

        if (!method.IsStatic)
        {
            // Method has `this` as its first parameter
            var methodParams = method.GetParameters();
            Parameters = [new ThisParameterInfo(method), ..methodParams];
        }
        else
        {
            Parameters = method.GetParameters();
        }

        Locals = ReflectionExtensions.GetLocals(method);
    }

    private object GetVariable(OpCode opCode, int index)
    {
        if (opCode.TargetsLocalVariable())
        {
            return Locals[index];
        }
        else
        {
            return Parameters[index];
        }
    }


    private OpCodeInstruction ReadOpCodeInstruction(ref SpanReader<byte> reader)
    {
        int offset = reader.Position;
        OpCode opCode = reader.ReadOpCode();

        switch (opCode.OperandType)
        {
            // operand is a 32-bit branch target
            case OperandType.InlineBrTarget:
            {
                int delta = reader.TakeValue<int>();
                return new OpCodeBranchInstruction(opCode, delta)
                {
                    Offset = offset,
                };
            }
            // operand is a 32-bit metadata token for a field
            case OperandType.InlineField:
            {
                int token = reader.TakeValue<int>();
                FieldInfo field = _tokenResolver.ResolveField(token, this.OwnerGenericTypes, this.MethodGenericTypes);
                return new OpCodeFieldInstruction(opCode, token)
                {
                    Offset = offset,
                    Field = field,
                };
            }
            // operand is a 32-bit integer
            case OperandType.InlineI:
            {
                Debug.Assert(opCode == OpCodes.Ldc_I4);
                int i32 = reader.TakeValue<int>();
                return new OpCodeI32Instruction(i32)
                {
                    Offset = offset,
                };
            }
            // operand is a 64-bit integer
            case OperandType.InlineI8:
            {
                Debug.Assert(opCode == OpCodes.Ldc_I8);
                long i64 = reader.TakeValue<long>();
                return new OpCodeI64Instruction(i64)
                {
                    Offset = offset,
                };
            }
            // The operand is a 32-bit metadata token for a method
            case OperandType.InlineMethod:
            {
                int token = reader.TakeValue<int>();
                MethodBase method = _tokenResolver.ResolveMethod(token, this.OwnerGenericTypes, this.MethodGenericTypes);
                return new OpCodeMethodInstruction(opCode, token)
                {
                    Offset = offset,
                    Method = method,
                };
            }
            // no operand
            case OperandType.InlineNone:
            {
                if (opCode.TargetsLocalVariable(out var index))
                {
                    var instruction = new OpCodeLocalInstruction(opCode, index.SomeOr(-1))
                    {
                        Offset = offset,
                    };
                    if (index.IsSome(out var i))
                    {
                        instruction.Local = new(Locals[i]);
                    }
                    return instruction;
                }
                else if (opCode.TargetsArgument(out index))
                {
                    var instruction = new OpCodeParameterInstruction(opCode, index.SomeOr(-1))
                    {
                        Offset = offset,
                    };
                    if (index.IsSome(out var i))
                    {
                        instruction.Parameter = Parameters[i];
                    }
                    return instruction;
                }
                else
                {
                    return new OpCodeInstruction(opCode)
                    {
                        Offset = offset,
                    };
                }
            }
            // operand is a 64-bit IEEE floating point number
            case OperandType.InlineR:
            {
                Debug.Assert(opCode == OpCodes.Ldc_I8);
                double f64 = reader.TakeValue<double>();
                return new OpCodeF64Instruction(f64)
                {
                    Offset = offset,
                };
            }
            // operand is a 32-bit metadata token for a signature
            case OperandType.InlineSig:
            {
                Debug.Assert(opCode == OpCodes.Calli);
                int token = reader.TakeValue<int>();
                byte[] sig = _tokenResolver.ResolveSignature(token);
                return new OpCodeSignatureInstruction(token)
                {
                    Offset = offset,
                    Signature = sig,
                };
            }
            // operand is a 32-bit metadata token for a string
            case OperandType.InlineString:
            {
                Debug.Assert(opCode == OpCodes.Ldstr);
                int token = reader.TakeValue<int>();
                string str = _tokenResolver.ResolveString(token);
                return new OpCodeStringInstruction(token)
                {
                    Offset = offset,
                    String = str,
                };
            }
            // operand is the 32-bit integer count of case offsets for a switch instruction
            case OperandType.InlineSwitch:
            {
                Debug.Assert(opCode == OpCodes.Switch);
                int cases = reader.TakeValue<int>();
                int[] deltas = new int[cases];
                for (int i = 0; i < cases; i++)
                {
                    deltas[i] = reader.TakeValue<int>();
                }
                return new OpCodeSwitchInstruction(deltas)
                {
                    Offset = offset,
                };
            }
            // operand is a 32-bit metadata token for a FieldRef, MethodRef, or TypeRef
            case OperandType.InlineTok:
            {
                Debug.Assert(opCode == OpCodes.Ldtoken);
                int token = reader.TakeValue<int>();
                MemberInfo member = _tokenResolver.ResolveMember(token, this.OwnerGenericTypes, this.MethodGenericTypes);
                return new OpCodeMemberInstruction(token)
                {
                    Offset = offset,
                    Member = member,
                };
            }
            // operand is a 32-bit metadata token for a Type
            case OperandType.InlineType:
            {
                int token = reader.TakeValue<int>();
                Type type = _tokenResolver.ResolveType(token, this.OwnerGenericTypes, this.MethodGenericTypes);
                return new OpCodeTypeInstruction(opCode, token)
                {
                    Offset = offset,
                    Type = type,
                };
            }
            // operand is 16-bit integer containing the index of a local variable or an argument
            case OperandType.InlineVar:
            {
                ushort index16 = reader.TakeValue<ushort>();
                if (opCode.TargetsLocalVariable(out var localIndex))
                {
                    var instruction = new OpCodeLocalInstruction(opCode, index16)
                    {
                        Offset = offset,
                    };
                    if (localIndex.IsSome(out var i))
                    {
                        instruction.Local = new(Locals[i]);
                    }
                    return instruction;
                }
                else if (opCode.TargetsArgument(out var argIndex))
                {
                    var instruction = new OpCodeParameterInstruction(opCode, index16)
                    {
                        Offset = offset,
                    };
                    if (argIndex.IsSome(out var i))
                    {
                        instruction.Parameter = Parameters[i];
                    }
                    return instruction;
                }
                throw new InvalidOperationException();
            }
            // operand is an 8-bit integer branch target
            case OperandType.ShortInlineBrTarget:
            {
                sbyte shortDelta = reader.TakeValue<sbyte>();
                return new OpCodeBranchInstruction(opCode, shortDelta)
                {
                    Offset = offset,
                };
            }
            // operand is a 8-bit integer
            case OperandType.ShortInlineI:
            {
                sbyte i8 = reader.TakeValue<sbyte>();
                return new OpCodeI8Instruction(opCode, i8)
                {
                    Offset = offset,
                };
            }
            // operand is a 32-bit IEEE floating point number
            case OperandType.ShortInlineR:
            {
                Debug.Assert(opCode == OpCodes.Ldc_R4);
                float f32 = reader.TakeValue<float>();
                return new OpCodeF32Instruction(f32)
                {
                    Offset = offset,
                };
            }
            // operand is 8-bit integer containing the index of a local variable or an argument
            case OperandType.ShortInlineVar:
            {
                byte index8 = reader.TakeValue<byte>();
                if (opCode.TargetsLocalVariable(out var localIndex))
                {
                    var instruction = new OpCodeLocalInstruction(opCode, index8)
                    {
                        Offset = offset,
                    };
                    if (localIndex.IsSome(out var i))
                    {
                        instruction.Local = new(Locals[i]);
                    }
                    return instruction;
                }
                else if (opCode.TargetsArgument(out var argIndex))
                {
                    var instruction = new OpCodeParameterInstruction(opCode, index8)
                    {
                        Offset = offset,
                    };
                    if (argIndex.IsSome(out var i))
                    {
                        instruction.Parameter = Parameters[i];
                    }
                    return instruction;
                }
                throw new InvalidOperationException();
            }
            default:
                throw InvalidEnumException.Create(opCode.OperandType);
        }
    }

    public InstructionStream ReadInstructions()
    {
        InstructionStream instructions = new();

        byte[] ilBytes = ReflectionExtensions.GetILBytes(_method);
        SpanReader<byte> reader = new(ilBytes);

        while (reader.RemainingCount > 0)
        {
            var instr = ReadOpCodeInstruction(ref reader);
            instructions.Add(instr);
        }

        return instructions;
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
}