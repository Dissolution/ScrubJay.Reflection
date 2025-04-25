using ScrubJay.Reflection.IL.Instructions;
using ScrubJay.Reflection.MosDef;

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
            GenericTypes = method.GetGenericArguments(),
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
                var instr = new OpCodeFieldInstruction(opCode, token)
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
                int i32 = reader.TakeValue<int>();
                return new OpCodeValueInstruction<int>(opCode, i32)
                {
                    Offset = offset,
                };
            }
            // operand is a 64-bit integer
            case OperandType.InlineI8:
            {
                Debug.Assert(opCode == OpCodes.Ldc_I8);
                long i64 = reader.TakeValue<long>();
                return new OpCodeValueInstruction<long>(opCode, i64)
                {
                    Offset = offset,
                };
            }
            // The operand is a 32-bit metadata token for a method
            case OperandType.InlineMethod:
            {
                int token = reader.TakeValue<int>();
                var instr = new OpCodeMethodInstruction(opCode, token)
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
                return new OpCodeNoneInstruction(opCode)
                {
                    Offset = offset,
                };
            }
            // operand is a 64-bit IEEE floating point number
            case OperandType.InlineR:
            {
                Debug.Assert(opCode == OpCodes.Ldc_R8);
                double f64 = reader.TakeValue<double>();
                return new OpCodeValueInstruction<double>(opCode, f64)
                {
                    Offset = offset,
                };
            }
            // operand is a 32-bit metadata token for a signature
            case OperandType.InlineSig:
            {
                Debug.Assert(opCode == OpCodes.Calli);
                int token = reader.TakeValue<int>();
                var instr = new OpCodeSignatureInstruction(opCode, token)
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
                int token = reader.TakeValue<int>();
                var instr = new OpCodeStringInstruction(opCode, token)
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
                int cases = reader.TakeValue<int>();
                int[] deltas = new int[cases];
                for (int i = 0; i < cases; i++)
                {
                    deltas[i] = reader.TakeValue<int>();
                }
                return new OpCodeSwitchInstruction(opCode, deltas)
                {
                    Offset = offset,
                };
            }
            // operand is a 32-bit metadata token for a FieldRef, MethodRef, or TypeRef
            case OperandType.InlineTok:
            {
                Debug.Assert(opCode == OpCodes.Ldtoken);
                int token = reader.TakeValue<int>();
                var instr = new OpCodeMemberInstruction(opCode, token)
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
                int token = reader.TakeValue<int>();
                var instr = new OpCodeTypeInstruction(opCode, token)
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
                ushort index = reader.TakeValue<ushort>();
                if (opCode.TargetsLocalVariable())
                {
                    return new OpCodeLocalInstruction(opCode, Locals[index])
                    {
                        Offset = offset,
                    };
                }
                else if (opCode.TargetsArgument())
                {
                    return new OpCodeParameterInstruction(opCode, Parameters[index])
                    {
                        Offset = offset,
                    };
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
                return new OpCodeValueInstruction<sbyte>(opCode, i8)
                {
                    Offset = offset,
                };
            }
            // operand is a 32-bit IEEE floating point number
            case OperandType.ShortInlineR:
            {
                Debug.Assert(opCode == OpCodes.Ldc_R4);
                float f32 = reader.TakeValue<float>();
                return new OpCodeValueInstruction<float>(opCode, f32)
                {
                    Offset = offset,
                };
            }
            // operand is 8-bit integer containing the index of a local variable or an argument
            case OperandType.ShortInlineVar:
            {
                byte index = reader.TakeValue<byte>();
                if (opCode.TargetsLocalVariable())
                {
                    return new OpCodeLocalInstruction(opCode, Locals[index])
                    {
                        Offset = offset,
                    };
                }
                else if (opCode.TargetsArgument())
                {
                    return new OpCodeParameterInstruction(opCode, Parameters[index])
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

        while (reader.RemainingCount > 0)
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


    public override string ToString()
    {
        return TextBuilder.New
            .AppendIf(MethodAttributes.HasFlags(MethodAttributes.Static), "static ")
            .AppendType(OwnerType)
            .Append('.')
            .AppendNameAndGenericTypes(Name, GenericTypes)
            .AppendLine('(')
            .EnumerateAndDelimit(Parameters, 
                static (tb, param) => tb.Append('[').Append(param.Position).Append("] ").AppendParameter(param),
                static tb => tb.Append(',').NewLine())
            .NewLine()
            .Append(") => ").AppendParameter(ReturnParameter).NewLine()
            .AppendLine("-- Locals")
            .Enumerate(Locals, (tb, local) => tb.Append(local.Index).Append(": ").Render(local).NewLine())
            .AppendLine("-- CIL")
            .LineDelimit(Instructions, (tb, instr) => instr.RenderTo(tb))
            .ToStringAndDispose();
    }
}