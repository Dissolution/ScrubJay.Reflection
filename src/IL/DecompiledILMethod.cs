using ScrubJay.Debugging;
using ScrubJay.Reflection.IL.Decompilation;
using ScrubJay.Reflection.IL.Instructions;
using ScrubJay.Reflection.MosDef;

namespace ScrubJay.Reflection.IL;

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
            Locals = locals,
            Labels = [],
            MethodAttributes = method.Attributes,
            Name = method.Name,
            TokenResolver = DecompileHelper.GetTokenResolver(method),
        };
        foreach (var instr in DecompileHelper.ReadInstructions(method))
        {
            dim.AddInstruction(instr);
        }

        return dim;
    }

    // hack
    public static Result<DecompiledILMethod> TryDecompile(MethodBase method)
    {
        return Result.TryInvoke(() => Decompile(method));
    }
    
    private OpCodeInstruction ReadNoArgInstruction(int offset, OpCode opCode)
    {
        Debug.Assert(opCode.OperandType == OperandType.InlineNone);

        // ldloc* stloc*
        if (opCode.TargetsLocal().Flatten().IsSome(out int index))
        {
            return new OpCodeLocalInstruction(opCode, index)
            {
                Offset = offset,
                Local = Locals[index],
            };
        }
        
        // ldarg* starg*
        if (opCode.TargetsArgument().Flatten().IsSome(out index))
        {
            return new OpCodeParameterInstruction(opCode, Parameters[index])
            {
                Offset = offset,
            };
        }
        
        // ldc.i4.*
        if (opCode.TargetsI32Const().IsSome(out var i32))
        {
            return new OpCodeValueInstruction<int>(opCode, i32)
            {
                Offset = offset,
            };
        }
        
        return new OpCodeInstruction(opCode)
        {
            Offset = offset,
        };
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
                FieldInfo field = ResolveField(token);
                return new OpCodeFieldInstruction(opCode, token)
                {
                    Offset = offset,
                    //Field = field,
                };
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
                //MethodBase method = _tokenResolver.ResolveMethod(token, this.OwnerGenericTypes, this.MethodGenericTypes);
                return new OpCodeMethodInstruction(opCode, token)
                {
                    Offset = offset,
                    //Method = method,
                };
            }
            // no operand
            case OperandType.InlineNone:
            {
                return ReadNoArgInstruction(offset, opCode);
            }
            // operand is a 64-bit IEEE floating point number
            case OperandType.InlineR:
            {
                Debug.Assert(opCode == OpCodes.Ldc_R8);
                double f64 = reader.TakeValue<double>();
                return new OpCodeValueInstruction<double>(opCode,f64)
                {
                    Offset = offset,
                };
            }
            // operand is a 32-bit metadata token for a signature
            case OperandType.InlineSig:
            {
                Debug.Assert(opCode == OpCodes.Calli);
                int token = reader.TakeValue<int>();
                //byte[] sig = _tokenResolver.ResolveSignature(token);
                return new OpCodeSignatureInstruction(opCode, token)
                {
                    Offset = offset,
                    //Signature = sig,
                };
            }
            // operand is a 32-bit metadata token for a string
            case OperandType.InlineString:
            {
                Debug.Assert(opCode == OpCodes.Ldstr);
                int token = reader.TakeValue<int>();
                //string str = _tokenResolver.ResolveString(token);
                return new OpCodeStringInstruction(token)
                {
                    Offset = offset,
                    //String = str,
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
                //MemberInfo member = _tokenResolver.ResolveMember(token, this.OwnerGenericTypes, this.MethodGenericTypes);
                return new OpCodeMemberInstruction(token)
                {
                    Offset = offset,
                    //Member = member,
                };
            }
            // operand is a 32-bit metadata token for a Type
            case OperandType.InlineType:
            {
                int token = reader.TakeValue<int>();
                //Type type = _tokenResolver.ResolveType(token, this.OwnerGenericTypes, this.MethodGenericTypes);
                return new OpCodeTypeInstruction(opCode, token)
                {
                    Offset = offset,
                    //Type = type,
                };
            }
            // operand is 16-bit integer containing the index of a local variable or an argument
            case OperandType.InlineVar:
            {
                ushort index = reader.TakeValue<ushort>();
                if (opCode.TargetsLocalVariable())
                {
                    return new OpCodeLocalInstruction(opCode, index)
                    {
                        Offset = offset,
                        //Local = new(Locals[index]),
                    };
                }
                else if (opCode.TargetsArgument())
                {
                    return new OpCodeParameterInstruction(opCode, index)
                    {
                        Offset = offset,
                        //Parameter = Parameters[index],
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
                    return new OpCodeLocalInstruction(opCode, index)
                    {
                        Offset = offset,
                    };
                }
                else if (opCode.TargetsArgument())
                {
                    return new OpCodeParameterInstruction(opCode, index)
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
    
    

    private InstructionStream ReadInstructions(MethodBase method)
    {
        InstructionStream instructions = new();

        byte[] ilBytes = DecompileHelper.GetILBytes(method);
        SpanReader<byte> reader = new(ilBytes);

        while (reader.RemainingCount > 0)
        {
            var instr = ReadOpCodeInstruction(ref reader);
            instructions.Add(instr);
        }

        return instructions;
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    public required ITokenResolver TokenResolver { get; init; }
    
    public required override IReadOnlyList<ILLocal> Locals { get;init;}
    
    public required override IReadOnlyList<ILLabel> Labels { get;init;}

    private FieldInfo? ResolveField(int token)
    {
        try
        {
            return TokenResolver.ResolveField(token, OwnerGenericTypes.NullIfNone(), GenericTypes.NullIfNone());
        }
        catch (Exception ex)
        {
            var dump = ex.Dump();
            Debugger.Break();
            return null;
        }
    }
   
    private MethodBase? ResolveMethod(int token)
    {
        try
        {
            return TokenResolver.ResolveMethod(token, OwnerGenericTypes.NullIfNone(), GenericTypes.NullIfNone());
        }
        catch (Exception ex)
        {
            var dump = ex.Dump();
            Debugger.Break();
            return null;
        }
    }
    
    private MemberInfo? ResolveMember(int token)
    {
        try
        {
            return TokenResolver.ResolveMember(token, OwnerGenericTypes.NullIfNone(), GenericTypes.NullIfNone());
        }
        catch (Exception ex)
        {
            var dump = ex.Dump();
            Debugger.Break();
            return null;
        }
    }
    
    private Type? ResolveType(int token)
    {
        try
        {
            return TokenResolver.ResolveType(token, OwnerGenericTypes.NullIfNone(), GenericTypes.NullIfNone());
        }
        catch (Exception ex)
        {
            var dump = ex.Dump();
            Debugger.Break();
            return null;
        }
    }
    
    private byte[]? ResolveSignature(int token)
    {
        try
        {
            return TokenResolver.ResolveSignature(token);
        }
        catch (Exception ex)
        {
            var dump = ex.Dump();
            Debugger.Break();
            return null;
        }
    }
    
    private string? ResolveString(int token)
    {
        try
        {
            return TokenResolver.ResolveString(token);
        }
        catch (Exception ex)
        {
            var dump = ex.Dump();
            Debugger.Break();
            return null;
        }
    }
    
    protected override OpCodeInstruction Inflate(OpCodeInstruction instruction)
    {
        instruction = base.Inflate(instruction);
        
        if (instruction is OpCodeFieldInstruction fieldInstr)
        {
            fieldInstr.Field = ResolveField(fieldInstr.Token);
        }
        else if (instruction is OpCodeMethodInstruction methodInstr)
        {
            methodInstr.Method = ResolveMethod(methodInstr.Token);
        }
        else if (instruction is OpCodeSignatureInstruction signatureInstr)
        {
            signatureInstr.Signature = ResolveSignature(signatureInstr.Token);
        }
        else if (instruction is OpCodeStringInstruction stringInstr)
        {
            stringInstr.String = ResolveString(stringInstr.Token);
        }
        else if (instruction is OpCodeMemberInstruction memberInstr)
        {
            memberInstr.Member = ResolveMember(memberInstr.Token);
        }
        else if (instruction is OpCodeTypeInstruction typeInstr)
        {
            typeInstr.Type = ResolveType(typeInstr.Token);
        }
        else
        {
            //Debugger.Break();
        }
        return instruction;
    }

    public override string ToString()
    {
        return TextBuilder.New
            .AppendIf(MethodAttributes.HasFlags(MethodAttributes.Static), "static ")
            .AppendType(OwnerType)
            .Append('.')
            .AppendNameAndGenericTypes(Name, GenericTypes)
            .AppendLine('(')
            .Delimit(static tb => tb.Append(',').NewLine(),
                Parameters,
                static (tb, param) => tb.Append('[').Append(param.Position).Append("] ").AppendParameter(param))
            .NewLine()
            .Append(") => ").AppendParameter(ReturnParameter).NewLine()
            .AppendLine("-- Locals")
            .Enumerate(Locals, (tb, local) => tb.Render(local).NewLine())
            .AppendLine("-- CIL")
            .LineDelimit(Instructions, (tb, instr) => instr.RenderTo(tb))
            .ToStringAndDispose();
    }
}