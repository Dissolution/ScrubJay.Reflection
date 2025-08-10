using ScrubJay.Reflection.IL.Instructions;

namespace ScrubJay.Reflection.IL.Decompilation;

public class DecompiledILMethod : ILMethod
{
    public static DecompiledILMethod Decompile(MethodBase method)
    {
        Throw.IfNull(method);

        var localVariables = DecompileHelper.GetLocals(method);
        int localCount = localVariables.Count;
        ILLocal[] locals = new ILLocal[localCount];
        for (var i = 0; i < localCount; i++)
        {
            locals[i] = new(localVariables[i]);
        }

        ParameterInfo returnParam;
        if (method is MethodInfo methodInfo)
        {
            returnParam = Result.TryInvoke(() => methodInfo.ReturnParameter).OkOrDefault()!;
            if (returnParam is null)
            {
                returnParam = new ReturnParameterInfo(methodInfo, methodInfo.ReturnType);
            }
        }
        else if (method is ConstructorInfo ctorInfo)
        {
            if (ctorInfo.IsStatic)
            {
                returnParam = new ReturnParameterInfo(ctorInfo, typeof(void));
            }
            else
            {
                returnParam = new ReturnParameterInfo(ctorInfo, ctorInfo.OwnerType());
            }
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(method));
        }

        ParameterInfo[] parameters = method.GetParameters();
        if (!method.IsStatic)
        {
            // the first parameter is actually `this`
            var thisParam = new ThisParameterInfo(method);
            parameters = [thisParam, ..parameters];
        }

        var dim = new DecompiledILMethod()
        {
            OwnerType = method.OwnerType(),
            GenericTypes = method.GenericTypes(),
            ReturnParameter = returnParam,
            Parameters = parameters,
            MethodAttributes = method.Attributes,
            Name = method.Name,
            TokenProvider = DecompileHelper.GetTokenResolver(method),
        };
        dim._locals.AddRange(locals);

        dim.ReadInstructions(method);
        return dim;
    }

    // hack
    public static Result<DecompiledILMethod> TryDecompile(MethodBase method)
    {
        return Result.TryInvoke(() => Decompile(method));
    }

    private OpCodeInstruction ReadOpCodeInstruction(ref SpanReader<byte> reader)
    {
        int offset = reader.Position;

        var readOp = reader.TryReadOpCode();
        if (!readOp.IsOk(out var opCode))
        {
            Debugger.Break();
        }

        switch (opCode.OperandType)
        {
            // operand is a 32-bit branch target
            case OperandType.InlineBrTarget:
            {
                int delta = reader.ReadI32();
                return new BranchInstruction(opCode, delta)
                {
                    Offset = offset,
                };
            }
            // operand is a 32-bit metadata token for a field
            case OperandType.InlineField:
            {
                int token = reader.ReadI32();
                var instr = new FieldInstruction(opCode, token)
                {
                    Offset = offset,
                };
                if (ResolveField(token).IsOk(out var field))
                {
                    instr.Field = field;
                }
                return instr;
            }
            // operand is a 32-bit integer
            case OperandType.InlineI:
            {
                Debug.Assert(opCode == OpCodes.Ldc_I4);
                int i32 = reader.ReadI32();
                return new ValueInstruction<int>(opCode, i32)
                {
                    Offset = offset,
                };
            }
            // operand is a 64-bit integer
            case OperandType.InlineI8:
            {
                Debug.Assert(opCode == OpCodes.Ldc_I8);
                long i64 = reader.ReadI64();
                return new ValueInstruction<long>(opCode, i64)
                {
                    Offset = offset,
                };
            }
            // The operand is a 32-bit metadata token for a method
            case OperandType.InlineMethod:
            {
                int token = reader.ReadI32();
                var instr = new MethodInstruction(opCode, token)
                {
                    Offset = offset,
                };
                if (ResolveMethod(token).IsOk(out var method))
                {
                    instr.Method = method;
                }
                return instr;
            }
            // no operand
            case OperandType.InlineNone:
            {
                return new NoneInstruction(opCode)
                {
                    Offset = offset,
                };
            }
            // operand is a 64-bit IEEE floating point number
            case OperandType.InlineR:
            {
                Debug.Assert(opCode == OpCodes.Ldc_R8);
                double f64 = reader.ReadF64();
                return new ValueInstruction<double>(opCode, f64)
                {
                    Offset = offset,
                };
            }
            // operand is a 32-bit metadata token for a signature
            case OperandType.InlineSig:
            {
                Debug.Assert(opCode == OpCodes.Calli);
                int token = reader.ReadI32();
                var instr = new SignatureInstruction(opCode, token)
                {
                    Offset = offset,
                };
                if (ResolveSignature(token).IsOk(out var sig))
                {
                    instr.Signature = sig;
                }
                return instr;
            }
            // operand is a 32-bit metadata token for a string
            case OperandType.InlineString:
            {
                Debug.Assert(opCode == OpCodes.Ldstr);
                int token = reader.ReadI32();
                var instr = new StringInstruction(opCode, token)
                {
                    Offset = offset,
                };
                if (ResolveString(token).IsOk(out var str))
                {
                    instr.String = str;
                }
                return instr;
            }
            // operand is the 32-bit integer count of case offsets for a switch instruction
            case OperandType.InlineSwitch:
            {
                Debug.Assert(opCode == OpCodes.Switch);
                int cases = reader.ReadI32();
                int[] deltas = new int[cases];
                for (int i = 0; i < cases; i++)
                {
                    deltas[i] = reader.ReadI32();
                }
                return new SwitchInstruction(opCode, deltas)
                {
                    Offset = offset,
                };
            }
            // operand is a 32-bit metadata token for a FieldRef, MethodRef, or TypeRef
            case OperandType.InlineTok:
            {
                Debug.Assert(opCode == OpCodes.Ldtoken);
                int token = reader.ReadI32();
                var instr = new MemberInstruction(opCode, token)
                {
                    Offset = offset,
                };
                if (ResolveMember(token).IsOk(out var member))
                {
                    instr.Member = member;
                }
                return instr;
            }
            // operand is a 32-bit metadata token for a Type
            case OperandType.InlineType:
            {
                int token = reader.ReadI32();
                var instr = new TypeInstruction(opCode, token)
                {
                    Offset = offset,
                };
                if (ResolveType(token).IsOk(out var type))
                {
                    instr.Type = type;
                }
                return instr;
            }
            // operand is 16-bit integer containing the index of a local variable or an argument
            case OperandType.InlineVar:
            {
                ushort index = reader.ReadU16();
                if (opCode.TargetsLocalVariable())
                {
                    return new LocalInstruction(opCode, Locals[index])
                    {
                        Offset = offset,
                    };
                }
                else if (opCode.TargetsArgument())
                {
                    return new ParameterInstruction(opCode, Parameters[index])
                    {
                        Offset = offset,
                    };
                }
                throw new InvalidOperationException();
            }
            // operand is an 8-bit integer branch target
            case OperandType.ShortInlineBrTarget:
            {
                sbyte shortDelta = reader.ReadI8();
                return new BranchInstruction(opCode, shortDelta)
                {
                    Offset = offset,
                };
            }
            // operand is a 8-bit integer
            case OperandType.ShortInlineI:
            {
                sbyte i8 = reader.ReadI8();
                return new ValueInstruction<sbyte>(opCode, i8)
                {
                    Offset = offset,
                };
            }
            // operand is a 32-bit IEEE floating point number
            case OperandType.ShortInlineR:
            {
                Debug.Assert(opCode == OpCodes.Ldc_R4);
                float f32 = reader.ReadF32();
                return new ValueInstruction<float>(opCode, f32)
                {
                    Offset = offset,
                };
            }
            // operand is 8-bit integer containing the index of a local variable or an argument
            case OperandType.ShortInlineVar:
            {
                byte index = reader.ReadU8();
                if (opCode.TargetsLocalVariable())
                {
                    return new LocalInstruction(opCode, Locals[index])
                    {
                        Offset = offset,
                    };
                }
                else if (opCode.TargetsArgument())
                {
                    return new ParameterInstruction(opCode, Parameters[index])
                    {
                        Offset = offset,
                    };
                }
                throw new InvalidOperationException();
            }
            default:
                throw InvalidEnumException.Create(opCode.OperandType);
        }
    }


    private void ReadInstructions(MethodBase method)
    {
        byte[] ilBytes = DecompileHelper.GetILBytes(method);
        SpanReader<byte> reader = new(ilBytes);

        while (!reader.IsCompleted)
        {
            var instr = ReadOpCodeInstruction(ref reader);
            AddInstruction(instr);
        }
    }


    public required ITokenProvider TokenProvider { get; init; }

    private Result<FieldInfo> ResolveField(int token)
    {
        return TokenProvider.ResolveField(token, OwnerGenericTypes.NullIfNone(), GenericTypes.NullIfNone());
    }

    private Result<MethodBase> ResolveMethod(int token)
    {
        return TokenProvider.ResolveMethod(token, OwnerGenericTypes.NullIfNone(), GenericTypes.NullIfNone());
    }

    private Result<MemberInfo> ResolveMember(int token)
    {
        return TokenProvider.ResolveMember(token, OwnerGenericTypes.NullIfNone(), GenericTypes.NullIfNone());
    }

    private Result<Type> ResolveType(int token)
    {
        return TokenProvider.ResolveType(token, OwnerGenericTypes.NullIfNone(), GenericTypes.NullIfNone());
    }

    private Result<byte[]> ResolveSignature(int token)
    {
        return TokenProvider.ResolveSignature(token);
    }

    private Result<string> ResolveString(int token)
    {
        return TokenProvider.ResolveString(token);
    }
}